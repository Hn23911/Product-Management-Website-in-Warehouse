using Microsoft.AspNetCore.Mvc;

namespace WebQuanLySanPhamKhoHang.Views.Shared.Components.SearchBar
{
    public class SearchBarViewComponent : ViewComponent
    {
        public SearchBarViewComponent()
        {
        }
        public IViewComponentResult Invoke(SPager SearchPager)
        {
            return View("SearchPager", SearchPager);
        }
    }
}
