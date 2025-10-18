using WebQuanLySanPhamKhoHang.Models;
using WebQuanLySanPhamKhoHang.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebQuanLySanPhamKhoHang.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class CategoryController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách Danh mục
        public async Task<IActionResult> Index()
        {
            var category = await _categoryRepository.GetAllAsync();
            return View(category);
        }
        // Hiển thị form thêm Danh mục mới
        public IActionResult Add()
        {
            return View();
        }
        // Xử lý thêm Danh mục mới

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            return View(category);
        }

        //xem Danh mục
        public async Task<IActionResult> Display(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        //update Danh mục
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));

            }
            return View(category);
        }
        //xóa Danh mục
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
