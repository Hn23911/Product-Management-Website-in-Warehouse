using WebQuanLySanPhamKhoHang.Data;
using WebQuanLySanPhamKhoHang.Models;
using WebQuanLySanPhamKhoHang.Repositories;
using WebQuanLySanPhamKhoHang.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebQuanLySanPhamKhoHang.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public EmployeeController(ApplicationDbContext context, IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        // Hiển thị danh sách Product

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
                    product = product.OrderBy(s => s.Name);
                //    product = product
                //.OrderByDescending(p => p.Status == "!!!")
                //.ThenByDescending(p => p.Status == "Hết hạn")
                //.ThenByDescending(p => p.Status == "Hết hàng")
                //.ThenBy(p => p.Name);
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
                Controller = "Employee",
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
                Controller = "Employee",
                SearchText = SearchText,
                Category = category, // Thêm tham số category vào để hiện url category

            };

            ViewBag.SearchPager = SearchPager;

            return View(retProducts);
        }


        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                await _productRepository.AddAsync(product);

                //// Lưu lịch sử hành động thêm sản phẩm
                var actionHistory = new ActionHistory
                {
                    ActionType = "Thêm",
                    ProductName = product.Name,
                    ProductId = product.Id,
                    UserId = User.Identity?.Name, // ID người thực hiện (admin hoặc employee)
                    ActionDate = DateTime.Now
                };
                _context.ActionHistories.Add(actionHistory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
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
        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name",
            product.CategoryId);
            return View(product);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await
                _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                     // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Price = product.Price;
                existingProduct.DateIn = product.DateIn;
                existingProduct.MFG = product.MFG;
                existingProduct.EXP = product.EXP;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                await _productRepository.UpdateAsync(existingProduct);

                // Lưu lịch sử hành động sửa sản phẩm
                var actionHistory = new ActionHistory
                {
                    ActionType = "Sửa",
                    ProductName = product.Name,
                    ProductId = product.Id,
                    UserId = User.Identity?.Name, // ID người thực hiện (admin hoặc employee)
                    ActionDate = DateTime.Now
                };
                _context.ActionHistories.Add(actionHistory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lưu lịch sử hành động xóa sản phẩm
            var actionHistory = new ActionHistory
            {
                ActionType = "Xóa",
                ProductName = product.Name,
                ProductId = product.Id,
                UserId = User.Identity?.Name, // ID người thực hiện (admin hoặc employee)
                ActionDate = DateTime.Now
            };
            _context.ActionHistories.Add(actionHistory);
            await _context.SaveChangesAsync();

            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ActionHistory()
        {
            var actionHistories = await _context.ActionHistories
                                                 .OrderByDescending(a => a.ActionDate)
                                                 .ToListAsync();
            return View(actionHistories);
        }
    }
}
