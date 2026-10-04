using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Staff;

public class HistoryModel : PageModel
{
    private readonly IApiService _apiService;

    public HistoryModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public List<NewsArticleViewModel> Articles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        var currentUser = SessionHelper.GetUser(HttpContext.Session);
        if (currentUser == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        Articles = await _apiService.GetMyNewsHistoryAsync(currentUser.AccountId);
        return Page();
    }
}
