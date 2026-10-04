using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public List<Category> GetCategories() => CategoryDAO.Instance.GetCategories();

    public List<Category> GetActiveCategories() => CategoryDAO.Instance.GetActiveCategories();

    public Category? GetCategoryById(short categoryId) => CategoryDAO.Instance.GetCategoryById(categoryId);

    public bool HasNewsArticles(short categoryId) => CategoryDAO.Instance.HasNewsArticles(categoryId);

    public bool CreateCategory(Category category) => CategoryDAO.Instance.CreateCategory(category);

    public bool UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);

    public bool DeleteCategory(short categoryId) => CategoryDAO.Instance.DeleteCategory(categoryId);
}
