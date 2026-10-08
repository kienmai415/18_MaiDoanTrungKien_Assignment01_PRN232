using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.DAOs;

public class NewsArticleDAO
{
    private static NewsArticleDAO? _instance;
    private static readonly object _instanceLock = new object();

    private NewsArticleDAO() { }

    public static NewsArticleDAO Instance
    {
        get
        {
            lock (_instanceLock)
            {
                _instance ??= new NewsArticleDAO();
                return _instance;
            }
        }
    }

    public List<NewsArticle> GetNewsArticles()
    {
        using var context = new FunewsManagementContext();
        return context.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public List<NewsArticle> GetActiveNewsArticles()
    {
        using var context = new FunewsManagementContext();
        return context.NewsArticles
            .Where(n => n.NewsStatus)
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    public NewsArticle? GetNewsArticleById(string id)
    {
        using var context = new FunewsManagementContext();
        return context.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.Tags)
            .AsNoTracking()
            .FirstOrDefault(n => n.NewsArticleId == id);
    }

    public List<NewsArticle> GetNewsArticlesByCreatedBy(short createdById)
    {
        using var context = new FunewsManagementContext();
        return context.NewsArticles
            .Where(n => n.CreatedById == createdById)
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.UpdatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    // =========================================================================================
    // [TRUY VẤN BÁO CÁO THỐNG KÊ (TRANG 4 ĐỀ BÀI)]:
    // "Create a report statistic by the period from StartDate to EndDate (it depends on the news' created date),
    // and sort data in descending order."
    // Thực thi LINQ trực tiếp xuống SQL Server:
    // 1. Where: Lọc bài viết có CreatedDate từ startDate đến hết ngày endDate.
    // 2. OrderByDescending: Sắp xếp giảm dần theo CreatedDate theo đúng quy định đề bài.
    // 3. Include: Eager loading nạp sẵn Category, CreatedBy, Tags.
    // =========================================================================================
    public List<NewsArticle> GetReportStatistics(DateTime startDate, DateTime endDate)
    {
        using var context = new FunewsManagementContext();
        var endOfDay = endDate.Date.AddDays(1).AddTicks(-1);
        return context.NewsArticles
            .Where(n => n.CreatedDate >= startDate.Date && n.CreatedDate <= endOfDay)
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags)
            .OrderByDescending(n => n.CreatedDate)
            .AsNoTracking()
            .ToList();
    }

    // =========================================================================================
    // [TẠO BÀI VIẾT & XỬ LÝ QUAN HỆ NHIỀU-NHIỀU VỚI BẢNG TAG]:
    // 1. Kiểm tra trùng lặp mã bài viết (NewsArticleId).
    // 2. Tự động thiết lập CreatedDate = DateTime.Now.
    // 3. Lấy các Tag tương ứng từ database dựa trên mảng tagIds và gán vào article.Tags.
    // 4. Entity Framework Core sẽ tự động insert dữ liệu vào bảng NewsArticle
    //    và các bản ghi tương ứng vào bảng trung gian NewsTag.
    // =========================================================================================
    public bool CreateNewsArticle(NewsArticle article, List<int>? tagIds)
    {
        using var context = new FunewsManagementContext();
        if (context.NewsArticles.Any(n => n.NewsArticleId == article.NewsArticleId))
        {
            throw new InvalidOperationException($"NewsArticle with ID '{article.NewsArticleId}' already exists.");
        }

        article.CreatedDate ??= DateTime.Now;
        article.Tags.Clear();

        if (tagIds != null && tagIds.Any())
        {
            var selectedTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
            foreach (var tag in selectedTags)
            {
                article.Tags.Add(tag);
            }
        }

        context.NewsArticles.Add(article);
        return context.SaveChanges() > 0;
    }

    public bool UpdateNewsArticle(NewsArticle article, List<int>? tagIds)
    {
        using var context = new FunewsManagementContext();
        var existing = context.NewsArticles
            .Include(n => n.Tags)
            .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId);

        if (existing == null)
        {
            return false;
        }

        existing.NewsTitle = article.NewsTitle;
        existing.Headline = article.Headline;
        existing.NewsContent = article.NewsContent;
        existing.NewsSource = article.NewsSource;
        existing.CategoryId = article.CategoryId;
        existing.NewsStatus = article.NewsStatus;
        existing.UpdatedById = article.UpdatedById;
        existing.ModifiedDate = DateTime.Now;

        // Update tags
        existing.Tags.Clear();
        if (tagIds != null && tagIds.Any())
        {
            var selectedTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
            foreach (var tag in selectedTags)
            {
                existing.Tags.Add(tag);
            }
        }

        return context.SaveChanges() > 0;
    }

    public bool DeleteNewsArticle(string id)
    {
        using var context = new FunewsManagementContext();
        var existing = context.NewsArticles
            .Include(n => n.Tags)
            .FirstOrDefault(n => n.NewsArticleId == id);

        if (existing == null)
        {
            return false;
        }

        context.NewsArticles.Remove(existing);
        return context.SaveChanges() > 0;
    }
}
