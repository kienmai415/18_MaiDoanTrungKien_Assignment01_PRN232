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

    // =========================================================================================
    // [LUỒNG TẢI DỮ LIỆU BÀI VIẾT]: Gọi khi Staff truy cập trang /Staff/NewsArticles
    // 1. Kiểm tra quyền Session (chỉ Staff hoặc Admin mới được xem).
    // 2. Tải danh sách Categories và Tags để nạp vào dropdown/checkbox trên popup modal.
    // 3. Tải danh sách Articles (có hỗ trợ tìm kiếm từ khóa Keyword).
    // Tiếp theo: Gọi ApiService.GetActiveCategoriesAsync(), GetTagsAsync(), GetNewsArticlesAsync()
    // =========================================================================================
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

    // =========================================================================================
    // [LUỒNG TẠO BÀI VIẾT MỚI & GÁN TAGS (TRANG 4 ĐỀ BÀI)]:
    // Kích hoạt khi Staff submit form trên Popup Dialog "Add News Article".
    // 1. Tự động lấy AccountId của Staff đang đăng nhập để gán vào CreatedById.
    // 2. Gom thông tin bài viết (ArticleForm) và danh sách thẻ được tích chọn (SelectedTags).
    // Tiếp theo: Gọi ApiService.CreateNewsArticleAsync(...) -> Gửi POST sang NewsArticlesController
    // =========================================================================================
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

    // =========================================================================================
    // [LUỒNG CẬP NHẬT BÀI VIẾT & CẬP NHẬT TAGS]:
    // Kích hoạt khi Staff chỉnh sửa trên Popup Dialog "Edit News Article".
    // Gán UpdatedById = tài khoản Staff hiện tại.
    // Tiếp theo: Gọi ApiService.UpdateNewsArticleAsync(...) -> Gửi PUT sang NewsArticlesController
    // =========================================================================================
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

    // =========================================================================================
    // [LUỒNG XÓA BÀI VIẾT (CÓ CONFIRM DIALOG)]:
    // Kích hoạt sau khi Staff bấm xác nhận xóa trên hộp thoại SweetAlert2.
    // Tiếp theo: Gọi ApiService.DeleteNewsArticleAsync(id) -> Gửi DELETE sang NewsArticlesController
    // BackEnd sẽ xóa bài viết và tự động cascade xóa các bản ghi liên kết trong bảng NewsTag.
    // =========================================================================================
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
