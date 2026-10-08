using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _18_MaiDoanTrungKien_FrontEnd.Helpers;
using _18_MaiDoanTrungKien_FrontEnd.Models;
using _18_MaiDoanTrungKien_FrontEnd.Services;

namespace _18_MaiDoanTrungKien_FrontEnd.Pages.Admin;

public class ReportsModel : PageModel
{
    private readonly IApiService _apiService;

    public ReportsModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);

    [BindProperty(SupportsGet = true)]
    public DateTime EndDate { get; set; } = DateTime.Today;

    public List<NewsArticleViewModel> Articles { get; set; } = new();

    public int TotalArticles => Articles.Count;
    public int ActiveArticles => Articles.Count(a => a.NewsStatus);
    public int InactiveArticles => Articles.Count(a => !a.NewsStatus);

    // =========================================================================================
    // [LUỒNG BÁO CÁO THỐNG KÊ (TRANG 4 ĐỀ BÀI)]:
    // "Create a report statistic by the period from StartDate to EndDate
    // (it depends on the news' created date), and sort data in descending order."
    // Luồng:
    // 1. Kiểm tra quyền Admin từ Session.
    // 2. Validate ràng buộc ngày: StartDate không được lớn hơn EndDate.
    // 3. Gọi ApiService.GetReportStatisticsAsync(StartDate, EndDate)
    //    -> Gửi GET api/NewsArticles/report?startDate=...&endDate=... sang BackEnd.
    // 4. BackEnd (NewsArticleDAO) truy vấn LINQ:
    //    Where(CreatedDate >= StartDate && CreatedDate <= EndDate)
    //    .OrderByDescending(CreatedDate) (Sắp xếp giảm dần theo ngày tạo).
    // 5. Tính toán tổng số bài (TotalArticles, ActiveArticles, InactiveArticles) và render bảng báo cáo.
    // =========================================================================================
    public async Task<IActionResult> OnGetAsync()
    {
        if (!SessionHelper.IsAdmin(HttpContext.Session))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }

        if (StartDate > EndDate)
        {
            TempData["ErrorMessage"] = "Start Date cannot be greater than End Date.";
            return Page();
        }

        Articles = await _apiService.GetReportStatisticsAsync(StartDate, EndDate);
        return Page();
    }
}
