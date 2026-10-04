using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Auth;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        SessionHelper.ClearUser(HttpContext.Session);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        return await OnPostAsync();
    }
}
