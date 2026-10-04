using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly IApiService _apiService;

    public IndexModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public List<NewsArticleViewModel> Articles { get; set; } = new();
    public List<CategoryViewModel> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchKeyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public short? SelectedCategoryId { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _apiService.GetActiveCategoriesAsync();
        Articles = await _apiService.GetActiveNewsArticlesAsync(SearchKeyword);

        if (SelectedCategoryId.HasValue && SelectedCategoryId.Value > 0)
        {
            Articles = Articles.FindAll(a => a.CategoryId == SelectedCategoryId.Value);
        }
    }
}
