using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

// =========================================================================================
// [TẦNG REPOSITORY CHO CATEGORIES (REPOSITORY PATTERN)]:
// Đóng vai trò trung gian giữa CategoriesController và CategoryDAO.
// Toàn bộ logic truy xuất đều gọi sang Singleton CategoryDAO.Instance.
// =========================================================================================
public class CategoryRepository : ICategoryRepository
{
    // Lấy danh sách toàn bộ chuyên mục (hỗ trợ OData trên Controller)
    public List<Category> GetCategories() => CategoryDAO.Instance.GetCategories();

    // Lấy danh sách chuyên mục đang hoạt động (IsActive = true)
    public List<Category> GetActiveCategories() => CategoryDAO.Instance.GetActiveCategories();

    // Lấy chuyên mục theo ID
    public Category? GetCategoryById(short categoryId) => CategoryDAO.Instance.GetCategoryById(categoryId);

    // Kiểm tra xem chuyên mục có bài viết trực thuộc không (dùng cho ràng buộc cấm xóa)
    public bool HasNewsArticles(short categoryId) => CategoryDAO.Instance.HasNewsArticles(categoryId);

    // Tạo mới chuyên mục (hỗ trợ quan hệ cha - con ParentCategoryId)
    public bool CreateCategory(Category category) => CategoryDAO.Instance.CreateCategory(category);

    // Cập nhật thông tin chuyên mục
    public bool UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);

    // Xóa chuyên mục (có kiểm tra ràng buộc không được xóa nếu có bài viết)
    public bool DeleteCategory(short categoryId) => CategoryDAO.Instance.DeleteCategory(categoryId);
}
