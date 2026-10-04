using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Staff;

public class ProfileModel : PageModel
{
    private readonly IApiService _apiService;

    public ProfileModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public AccountViewModel ProfileForm { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var currentUser = SessionHelper.GetUser(HttpContext.Session);
        if (currentUser == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        var profile = await _apiService.GetAccountByIdAsync(currentUser.AccountId);
        if (profile != null)
        {
            ProfileForm = profile;
        }
        else
        {
            ProfileForm.AccountId = currentUser.AccountId;
            ProfileForm.AccountName = currentUser.AccountName;
            ProfileForm.AccountEmail = currentUser.AccountEmail;
            ProfileForm.AccountRole = currentUser.AccountRole;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var currentUser = SessionHelper.GetUser(HttpContext.Session);
        if (currentUser == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        if (string.IsNullOrWhiteSpace(ProfileForm.AccountName) || string.IsNullOrWhiteSpace(ProfileForm.AccountEmail))
        {
            TempData["ErrorMessage"] = "Name and Email cannot be empty.";
            return Page();
        }

        ProfileForm.AccountId = currentUser.AccountId;
        ProfileForm.AccountRole = currentUser.AccountRole;

        var (success, message) = await _apiService.UpdateAccountAsync(ProfileForm);
        if (success)
        {
            currentUser.AccountName = ProfileForm.AccountName;
            currentUser.AccountEmail = ProfileForm.AccountEmail;
            SessionHelper.SetUser(HttpContext.Session, currentUser);
            TempData["SuccessMessage"] = "Profile updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToPage();
    }
}
