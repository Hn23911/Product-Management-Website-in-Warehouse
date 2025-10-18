using Microsoft.AspNetCore.Mvc;
using WebQuanLySanPhamKhoHang.Repositories;

namespace WebQuanLySanPhamKhoHang.Views.Shared.Components.listCategory
{
    public class listCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public listCategoryViewComponent(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _categoryRepository.GetAllDanhMucAsync().OrderBy(x => x.Name);
            return View("listCategory");
        }
    }
}
