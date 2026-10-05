using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IApiService _apiService;

    public LoginModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        if (SessionHelper.GetUser(HttpContext.Session) != null)
        {
            var user = SessionHelper.GetUser(HttpContext.Session);
            if (user?.RoleName == "Admin")
            {
                Response.Redirect("/Admin/Accounts");
            }
            else
            {
                Response.Redirect("/Staff/NewsArticles");
            }
        }
    }

    // =========================================================================================
    // [BƯỚC 1 - GIAO DIỆN LOGIN]: Người dùng submit form đăng nhập (Email + Password).
    // Hàm này là điểm bắt đầu (Entry Point) của luồng đăng nhập phía FrontEnd.
    // Tiếp theo: Gọi hàm ApiService.LoginAsync(...) trong class ApiService.
    // =========================================================================================
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // [BƯỚC 2]: Gọi ApiService.LoginAsync() để gửi HTTP POST request sang BackEnd Web API
        var (success, user, error) = await _apiService.LoginAsync(Input.Email, Input.Password);
        if (!success || user == null)
        {
            ErrorMessage = error;
            return Page();
        }

        // [BƯỚC 8 - NHẬN KẾT QUẢ TỪ API TRẢ VỀ]: Lưu thông tin User vào Session thông qua SessionHelper
        SessionHelper.SetUser(HttpContext.Session, user);

        // Thiết lập Cookie Authentication để duy trì trạng thái đăng nhập
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
            new(ClaimTypes.Name, user.AccountName),
            new(ClaimTypes.Email, user.AccountEmail),
            new(ClaimTypes.Role, user.RoleName)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            });

        TempData["SuccessMessage"] = $"Welcome back, {user.AccountName} ({user.RoleName})!";

        // [BƯỚC 9 - ĐIỀU HƯỚNG THEO ROLE]: 
        // Admin chuyển hướng sang trang quản lý tài khoản (/Admin/Accounts)
        // Staff chuyển hướng sang trang quản lý tin tức (/Staff/NewsArticles)
        if (user.RoleName == "Admin")
        {
            return RedirectToPage("/Admin/Accounts");
        }
        else if (user.RoleName == "Staff")
        {
            return RedirectToPage("/Staff/NewsArticles");
        }

        return RedirectToPage("/Index");
    }
}
