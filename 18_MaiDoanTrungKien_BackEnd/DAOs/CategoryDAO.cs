using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.DAOs;

public class CategoryDAO
{
    private static CategoryDAO? _instance;
    private static readonly object _instanceLock = new object();

    private CategoryDAO() { }

    public static CategoryDAO Instance
    {
        get
        {
            lock (_instanceLock)
            {
                _instance ??= new CategoryDAO();
                return _instance;
            }
        }
    }

    public List<Category> GetCategories()
    {
        using var context = new FunewsManagementContext();
        return context.Categories
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .ToList();
    }

    public List<Category> GetActiveCategories()
    {
        using var context = new FunewsManagementContext();
        return context.Categories
            .Where(c => c.IsActive)
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .ToList();
    }

    public Category? GetCategoryById(short categoryId)
    {
        using var context = new FunewsManagementContext();
        return context.Categories
            .Include(c => c.ParentCategory)
            .AsNoTracking()
            .FirstOrDefault(c => c.CategoryId == categoryId);
    }

    public bool HasNewsArticles(short categoryId)
    {
        using var context = new FunewsManagementContext();
        return context.NewsArticles.Any(n => n.CategoryId == categoryId);
    }

    public bool CreateCategory(Category category)
    {
        using var context = new FunewsManagementContext();
        context.Categories.Add(category);
        return context.SaveChanges() > 0;
    }

    public bool UpdateCategory(Category category)
    {
        using var context = new FunewsManagementContext();
        var existing = context.Categories.Find(category.CategoryId);
        if (existing == null)
        {
            return false;
        }

        existing.CategoryName = category.CategoryName;
        existing.CategoryDesciption = category.CategoryDesciption;
        existing.ParentCategoryId = category.ParentCategoryId;
        existing.IsActive = category.IsActive;

        return context.SaveChanges() > 0;
    }

    public bool DeleteCategory(short categoryId)
    {
        using var context = new FunewsManagementContext();
        var existing = context.Categories.Find(categoryId);
        if (existing == null)
        {
            return false;
        }

        if (HasNewsArticles(categoryId))
        {
            throw new InvalidOperationException("Cannot delete this category because it has news articles associated with it.");
        }

        var hasChildCategories = context.Categories.Any(c => c.ParentCategoryId == categoryId);
        if (hasChildCategories)
        {
            throw new InvalidOperationException("Cannot delete this category because it has child subcategories.");
        }

        context.Categories.Remove(existing);
        return context.SaveChanges() > 0;
    }
}
