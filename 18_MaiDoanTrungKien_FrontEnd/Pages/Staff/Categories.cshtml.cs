using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Staff;

public class CategoriesModel : PageModel
{
    private readonly IApiService _apiService;

    public CategoriesModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public List<CategoryViewModel> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty]
    public CategoryViewModel CategoryForm { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        Categories = await _apiService.GetCategoriesAsync(Keyword);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        if (string.IsNullOrWhiteSpace(CategoryForm.CategoryName) || string.IsNullOrWhiteSpace(CategoryForm.CategoryDesciption))
        {
            TempData["ErrorMessage"] = "Name and Description are required.";
            return RedirectToPage(new { keyword = Keyword });
        }

        var (success, message) = await _apiService.CreateCategoryAsync(CategoryForm);
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

        if (string.IsNullOrWhiteSpace(CategoryForm.CategoryName) || string.IsNullOrWhiteSpace(CategoryForm.CategoryDesciption))
        {
            TempData["ErrorMessage"] = "Name and Description are required.";
            return RedirectToPage(new { keyword = Keyword });
        }

        var (success, message) = await _apiService.UpdateCategoryAsync(CategoryForm);
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
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        var (success, message) = await _apiService.DeleteCategoryAsync(id);
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
