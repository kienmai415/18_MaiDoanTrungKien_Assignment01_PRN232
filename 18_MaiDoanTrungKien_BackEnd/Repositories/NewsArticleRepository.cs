using System;
using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public class NewsArticleRepository : INewsArticleRepository
{
    public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.Instance.GetNewsArticles();

    public List<NewsArticle> GetActiveNewsArticles() => NewsArticleDAO.Instance.GetActiveNewsArticles();

    public NewsArticle? GetNewsArticleById(string id) => NewsArticleDAO.Instance.GetNewsArticleById(id);

    public List<NewsArticle> GetNewsArticlesByCreatedBy(short createdById) => NewsArticleDAO.Instance.GetNewsArticlesByCreatedBy(createdById);

    public List<NewsArticle> GetReportStatistics(DateTime startDate, DateTime endDate) => NewsArticleDAO.Instance.GetReportStatistics(startDate, endDate);

    public bool CreateNewsArticle(NewsArticle article, List<int>? tagIds) => NewsArticleDAO.Instance.CreateNewsArticle(article, tagIds);

    public bool UpdateNewsArticle(NewsArticle article, List<int>? tagIds) => NewsArticleDAO.Instance.UpdateNewsArticle(article, tagIds);

    public bool DeleteNewsArticle(string id) => NewsArticleDAO.Instance.DeleteNewsArticle(id);
}
