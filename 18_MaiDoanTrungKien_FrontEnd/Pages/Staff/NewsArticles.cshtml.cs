using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Staff;

public class NewsArticlesModel : PageModel
{
    private readonly IApiService _apiService;

    public NewsArticlesModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public List<NewsArticleViewModel> Articles { get; set; } = new();
    public List<CategoryViewModel> Categories { get; set; } = new();
    public List<TagViewModel> Tags { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty]
    public NewsArticleViewModel ArticleForm { get; set; } = new();

    [BindProperty]
    public List<int> SelectedTags { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        Categories = await _apiService.GetActiveCategoriesAsync();
        Tags = await _apiService.GetTagsAsync();
        Articles = await _apiService.GetNewsArticlesAsync(Keyword);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        var currentUser = SessionHelper.GetUser(HttpContext.Session);
        ArticleForm.CreatedById = currentUser != null && currentUser.AccountId > 0 ? currentUser.AccountId : null;

        if (string.IsNullOrWhiteSpace(ArticleForm.NewsArticleId) || string.IsNullOrWhiteSpace(ArticleForm.NewsTitle))
        {
            TempData["ErrorMessage"] = "Article ID and Title are required.";
            return RedirectToPage(new { keyword = Keyword });
        }

        var (success, message) = await _apiService.CreateNewsArticleAsync(ArticleForm, SelectedTags);
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
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        var currentUser = SessionHelper.GetUser(HttpContext.Session);
        ArticleForm.UpdatedById = currentUser != null && currentUser.AccountId > 0 ? currentUser.AccountId : null;

        if (string.IsNullOrWhiteSpace(ArticleForm.NewsArticleId) || string.IsNullOrWhiteSpace(ArticleForm.NewsTitle))
        {
            TempData["ErrorMessage"] = "Article ID and Title are required.";
            return RedirectToPage(new { keyword = Keyword });
        }

        var (success, message) = await _apiService.UpdateNewsArticleAsync(ArticleForm, SelectedTags);
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

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        var (success, message) = await _apiService.DeleteNewsArticleAsync(id);
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
