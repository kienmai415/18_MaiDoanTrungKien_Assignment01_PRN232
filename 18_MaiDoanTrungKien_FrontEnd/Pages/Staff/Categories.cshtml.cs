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

    // =========================================================================================
    // [LUỒNG TẢI DANH MỤC]: Gọi khi Staff truy cập trang /Staff/Categories
    // Tải danh sách chuyên mục (hỗ trợ lọc từ khóa Keyword).
    // Tiếp theo: Gọi ApiService.GetCategoriesAsync(Keyword) -> GET api/Categories
    // =========================================================================================
    public async Task<IActionResult> OnGetAsync()
    {
        if (!SessionHelper.IsStaff(HttpContext.Session) && !SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        Categories = await _apiService.GetCategoriesAsync(Keyword);
        return Page();
    }

    // =========================================================================================
    // [LUỒNG TẠO DANH MỤC MỚI]: Kích hoạt khi Staff submit Modal "Add Category".
    // Hỗ trợ chọn ParentCategoryID để tạo quan hệ phân cấp cha - con.
    // Tiếp theo: Gọi ApiService.CreateCategoryAsync(...) -> POST api/Categories
    // =========================================================================================
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

    // =========================================================================================
    // [LUỒNG CẬP NHẬT DANH MỤC]: Kích hoạt khi Staff sửa thông tin trên Modal "Edit Category".
    // Tiếp theo: Gọi ApiService.UpdateCategoryAsync(...) -> PUT api/Categories/{id}
    // =========================================================================================
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

    // =========================================================================================
    // [LUỒNG XÓA DANH MỤC & RÀNG BUỘC NGHIỆP VỤ (TRANG 4 ĐỀ BÀI)]:
    // "The delete action will delete an item in case this item has not belonged to any news articles.
    // If the item is already stored in a news article cannot delete."
    // Luồng:
    // 1. Staff xác nhận xóa trên hộp thoại SweetAlert2.
    // 2. Gọi ApiService.DeleteCategoryAsync(id) -> Gửi DELETE api/Categories/{id}.
    // 3. Phía BackEnd (CategoryDAO) kiểm tra xem có bài viết nào thuộc danh mục này không.
    //    Nếu có bài -> Chặn xóa và trả về thông báo lỗi 400.
    // =========================================================================================
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
