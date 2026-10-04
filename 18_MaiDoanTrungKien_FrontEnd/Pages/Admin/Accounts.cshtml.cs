using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Admin;

public class AccountsModel : PageModel
{
    private readonly IApiService _apiService;

    public AccountsModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public List<AccountViewModel> Accounts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty]
    public AccountViewModel AccountForm { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        Accounts = await _apiService.GetAccountsAsync(Keyword);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        if (string.IsNullOrWhiteSpace(AccountForm.AccountEmail) || string.IsNullOrWhiteSpace(AccountForm.AccountPassword))
        {
            TempData["ErrorMessage"] = "Email and Password are required.";
            return RedirectToPage(new { keyword = Keyword });
        }

        var (success, message) = await _apiService.CreateAccountAsync(AccountForm);
        if (success)
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToPage(new { keyword = Keyword });
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        if (string.IsNullOrWhiteSpace(AccountForm.AccountEmail) || string.IsNullOrWhiteSpace(AccountForm.AccountName))
        {
            TempData["ErrorMessage"] = "Name and Email are required.";
            return RedirectToPage(new { keyword = Keyword });
        }

        var (success, message) = await _apiService.UpdateAccountAsync(AccountForm);
        if (success)
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToPage(new { keyword = Keyword });
    }

    public async Task<IActionResult> OnPostDeleteAsync(short id)
    {
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        var (success, message) = await _apiService.DeleteAccountAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToPage(new { keyword = Keyword });
    }
}
