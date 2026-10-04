using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public interface ICategoryRepository
{
    List<Category> GetCategories();
    List<Category> GetActiveCategories();
    Category? GetCategoryById(short categoryId);
    bool HasNewsArticles(short categoryId);
    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(short categoryId);
}
