using System;
using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

// =========================================================================================
// [TẦNG REPOSITORY CHO NEWS ARTICLES (REPOSITORY PATTERN)]:
// Đóng vai trò trung gian giữa NewsArticlesController và NewsArticleDAO.
// Toàn bộ logic truy xuất đều gọi sang Singleton NewsArticleDAO.Instance.
// =========================================================================================
public class NewsArticleRepository : INewsArticleRepository
{
    // Lấy toàn bộ bài viết (hỗ trợ OData trên Controller)
    public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.Instance.GetNewsArticles();

    // Lấy bài viết trạng thái Active (cho khách xem ở Trang chủ)
    public List<NewsArticle> GetActiveNewsArticles() => NewsArticleDAO.Instance.GetActiveNewsArticles();

    // Lấy chi tiết bài viết theo mã NewsArticleId
    public NewsArticle? GetNewsArticleById(string id) => NewsArticleDAO.Instance.GetNewsArticleById(id);

    // Lấy lịch sử bài viết do tài khoản Staff tạo
    public List<NewsArticle> GetNewsArticlesByCreatedBy(short createdById) => NewsArticleDAO.Instance.GetNewsArticlesByCreatedBy(createdById);

    // Lấy danh sách bài viết báo cáo theo khoảng ngày (sắp xếp giảm dần theo ngày tạo)
    public List<NewsArticle> GetReportStatistics(DateTime startDate, DateTime endDate) => NewsArticleDAO.Instance.GetReportStatistics(startDate, endDate);

    // Tạo bài viết mới kèm danh sách thẻ Tags
    public bool CreateNewsArticle(NewsArticle article, List<int>? tagIds) => NewsArticleDAO.Instance.CreateNewsArticle(article, tagIds);

    // Cập nhật bài viết và đồng bộ lại thẻ Tags
    public bool UpdateNewsArticle(NewsArticle article, List<int>? tagIds) => NewsArticleDAO.Instance.UpdateNewsArticle(article, tagIds);

    // Xóa bài viết
    public bool DeleteNewsArticle(string id) => NewsArticleDAO.Instance.DeleteNewsArticle(id);
}
