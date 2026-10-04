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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (success, user, error) = await _apiService.LoginAsync(Input.Email, Input.Password);
        if (!success || user == null)
        {
            ErrorMessage = error;
            return Page();
        }

        // Set session
        SessionHelper.SetUser(HttpContext.Session, user);

        // Set Cookie Authentication
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
