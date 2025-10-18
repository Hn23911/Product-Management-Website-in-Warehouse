using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebQuanLySanPhamKhoHang.Models;

namespace WebQuanLySanPhamKhoHang.Areas.Identity.Pages.Account
{
    public class NotificationEmailConfirmModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationEmailConfirmModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public bool IsEmailConfirmed { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            }
            else
            {
                // Handle the case where user is not found
            }

            // Kiểm tra xem email đã được xác nhận hay chưa
            if (IsEmailConfirmed)
            {
                // Nếu đã xác nhận email, chuyển hướng đến trang chính
                return RedirectToPage("/Index");
            }
            else
            {
                // Nếu chưa xác nhận email, hiển thị trang thông báo và yêu cầu xác nhận
                return Page();
            }
        }
    }
}
