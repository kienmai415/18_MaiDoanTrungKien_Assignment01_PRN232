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

    // =========================================================================================
    // [LUỒNG XEM TIN TỨC CHO KHÁCH (TRANG 4 ĐỀ BÀI)]:
    // "Do not need authentication to view the news article (news status must be active) in this system."
    // Luồng:
    // 1. Khách vãng lai truy cập trang chủ /Index không cần đăng nhập.
    // 2. Tải danh sách chuyên mục đang hoạt động (Active Categories) vào bộ lọc dropdown.
    // 3. Tải danh sách bài viết CHỈ CÓ TRẠNG THÁI ACTIVE (NewsStatus = true).
    //    Gọi ApiService.GetActiveNewsArticlesAsync(SearchKeyword) -> GET api/NewsArticles/active.
    // 4. Nếu người dùng chọn lọc theo chuyên mục (SelectedCategoryId) -> Tiến hành lọc danh sách tương ứng.
    // =========================================================================================
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
