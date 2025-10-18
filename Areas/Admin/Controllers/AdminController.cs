using WebQuanLySanPhamKhoHang.Data;
using WebQuanLySanPhamKhoHang.Models;
using WebQuanLySanPhamKhoHang.Repositories;
using WebQuanLySanPhamKhoHang.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace WebQuanLySanPhamKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ApplicationDbContext context, IProductRepository productRepository,
        ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger)
        {
            _context = context;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        //Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(string sortOrder, string SearchText = "", int pg = 1)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateInSortParm"] = sortOrder == "datein" ? "datein_desc" : "datein";
            ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["MFGSortParm"] = sortOrder == "mfg" ? "mfg_desc" : "mfg";
            ViewData["EXPSortParm"] = sortOrder == "exp" ? "exp_desc" : "exp";

            var product = await _productRepository.GetAllAsync();

            foreach (var products in product)
            {

                if (products.Quantity <= 50 && products.EXP.HasValue && products.EXP.Value <= DateTime.Now.AddDays(7))
                {
                    products.Status = "!!!";
                }
                else if (products.Quantity <= 50)
                {
                    products.Status = "Hết hàng";
                }
                else if (products.EXP.HasValue && products.EXP.Value <= DateTime.Now.AddDays(7))
                {
                    products.Status = "Hết hạn";
                }
                else
                {
                    products.Status = "";
                }
            }

            // Sắp xếp các sản phẩm "Sắp hết hạn" lên đầu danh sách
            

            switch (sortOrder)
            {
                case "name_desc":
                    product = product.OrderByDescending(s => s.Name);
                    break;
                case "price":
                    product = product.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    product = product.OrderByDescending(s => s.Price);
                    break;
                case "datein":
                    product = product.OrderBy(s => s.DateIn);
                    break;
                case "datein_desc":
                    product = product.OrderByDescending(s => s.DateIn);
                    break;
                case "mfg":
                    product = product.OrderBy(s => s.MFG);
                    break;
                case "mfg_desc":
                    product = product.OrderByDescending(s => s.MFG);
                    break;
                case "exp":
                    product = product.OrderBy(s => s.EXP);
                    break;
                case "exp_desc":
                    product = product.OrderByDescending(s => s.EXP);
                    break;
                default:
                    //product = product.OrderBy(s => s.Name);
                    product = product
                .OrderByDescending(p => p.Status == "!!!")
                .ThenByDescending(p => p.Status == "Hết hạn")
                .ThenByDescending(p => p.Status == "Hết hàng")
                .ThenBy(p => p.Name);
                    break;
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                product = product.Where(x => x.Name.Contains(SearchText));
            }

            const int pageSize = 12;//số sp hiện trong 1 trang
            if (pg < 1)
                pg = 1;

            int resCount = product.Count();
            int recSkip = (pg - 1) * pageSize;
            var retProducts = product.Skip(recSkip).Take(pageSize).ToList();

            SPager SearchPager = new SPager(resCount, pg, pageSize)
            {
                Action = "Index",
                Controller = "Admin",
                SearchText = SearchText,
                Sort = sortOrder,
            };
            ViewBag.SearchPager = SearchPager;
            return View(retProducts);
        }

        public async Task<IActionResult> productsByCategories(string category, string SearchText = "", int pg = 1)
        {
            var list = await _productRepository.GetAllSPAsync(category);

            const int pageSize = 12; // số sản phẩm hiện trong 1 trang
            if (pg < 1)
                pg = 1;

            int resCount = list.Count();
            int recSkip = (pg - 1) * pageSize;
            List<Product> retProducts = list.Skip(recSkip).Take(pageSize).ToList();

            SPager SearchPager = new SPager(resCount, pg, pageSize)
            {
                Action = "productsByCategories",
                Controller = "Admin",
                SearchText = SearchText,
                Category = category, // Thêm tham số category vào để hiện url category

            };

            ViewBag.SearchPager = SearchPager;

            return View(retProducts);
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        public IActionResult Users()
        {
            var allUsers = _userManager.Users.ToList();

            // Lọc ra tất cả các tài khoản không phải là admin
            var nonAdminUsers = new List<ApplicationUser>();
            foreach (var user in allUsers)
            {
                if (!IsUserAdmin(user.Id).Result)
                {
                    nonAdminUsers.Add(user);
                }
            }
            return View(nonAdminUsers);
        }

        // cập nhật role
        [HttpGet]
        public async Task<IActionResult> EditRole(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Người dùng '{email}' không tồn tại.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();


            ViewBag.Roles = allRoles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = userRoles.Contains(r.Name)
            }).ToList();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(string email, string selectedRoleName)
        {
            try
            {
                // Debug: Kiểm tra giá trị của email
                if (string.IsNullOrEmpty(email))
                {
                    Debug.WriteLine("Email không tồn tại hoặc trống.");
                    return BadRequest("Email không tồn tại hoặc trống.");
                }
                else
                {
                    Debug.WriteLine($"Email: {email}");
                }

                // Debug: Kiểm tra giá trị của selectedRoleName
                if (string.IsNullOrEmpty(selectedRoleName))
                {
                    Debug.WriteLine("Tên role đã chọn không tồn tại hoặc trống.");
                    return BadRequest($"Tên role đã chọn: {selectedRoleName}");
                }
                else
                {
                    Debug.WriteLine($"Tên role đã chọn: {selectedRoleName}");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound($"Người dùng '{email}' không tồn tại.");
                }

                var role = await _roleManager.FindByIdAsync(selectedRoleName);
                if (role == null)
                {
                    return BadRequest($"Role '{selectedRoleName}' không tồn tại.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());

                var result = await _userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Cập nhật role thành công cho email: {email}");
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi cập nhật role cho email {email}: {ex.Message}");
                ModelState.AddModelError("", "Lỗi cập nhật role.");
            }

            var userToUpdate = await _userManager.FindByEmailAsync(email);
            if (userToUpdate != null)
            {
                var userRolesToUpdate = await _userManager.GetRolesAsync(userToUpdate);
                ViewBag.Roles = _roleManager.Roles.ToList().Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = userRolesToUpdate.Contains(r.Name)
                });
            }
            else
            {
                ModelState.AddModelError("", $"Người dùng '{email}' không tồn tại.");
            }

            return View(userToUpdate);
        }
        //khóa user vĩnh viễn
        [HttpPost]
        public async Task<IActionResult> LockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Người dùng '{email}' không tồn tại.");
            }

            var lockoutEndDate = DateTimeOffset.MaxValue; // lock vĩnh viễn
            await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            _logger.LogInformation($"Người dùng email '{email}' sẽ bị cấm tới {lockoutEndDate}.");
            return RedirectToAction("Users");
        }
        //mở khóa user
        [HttpPost]
        public async Task<IActionResult> UnlockUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Người dùng '{email}' không tồn tại.");
            }

            await _userManager.SetLockoutEndDateAsync(user, null);  // Bỏ lock user

            _logger.LogInformation($"Người dùng email '{email}' đã được mở cấm.");
            return RedirectToAction("Users");
        }
        //kiểm tra user là admin
        private async Task<bool> IsUserAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }
        public IActionResult UpdateComplete()
        {
            return View("UpdateComplete");
        }
        //ghi lại lịch sử thao tác
        public async Task<IActionResult> ActionHistory()
        {
            var actionHistories = await _context.ActionHistories
                                                 .OrderByDescending(a => a.ActionDate)
                                                 .ToListAsync();
            return View(actionHistories);
        }
    }
}
