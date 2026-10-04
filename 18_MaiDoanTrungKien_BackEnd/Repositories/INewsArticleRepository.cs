using System;
using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public interface INewsArticleRepository
{
    List<NewsArticle> GetNewsArticles();
    List<NewsArticle> GetActiveNewsArticles();
    NewsArticle? GetNewsArticleById(string id);
    List<NewsArticle> GetNewsArticlesByCreatedBy(short createdById);
    List<NewsArticle> GetReportStatistics(DateTime startDate, DateTime endDate);
    bool CreateNewsArticle(NewsArticle article, List<int>? tagIds);
    bool UpdateNewsArticle(NewsArticle article, List<int>? tagIds);
    bool DeleteNewsArticle(string id);
}
