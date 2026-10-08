using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using _18_MaiDoanTrungKien_BackEnd.DTOs;
using _18_MaiDoanTrungKien_BackEnd.Models;
using _18_MaiDoanTrungKien_BackEnd.Repositories;

namespace _18_MaiDoanTrungKien_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    [EnableQuery]
    public ActionResult<IEnumerable<Category>> GetCategories()
    {
        var categories = _categoryRepository.GetCategories();
        return Ok(categories);
    }

    [HttpGet("active")]
    public ActionResult<IEnumerable<Category>> GetActiveCategories()
    {
        var activeCategories = _categoryRepository.GetActiveCategories();
        return Ok(activeCategories);
    }

    [HttpGet("{id}")]
    public ActionResult<Category> GetCategory(short id)
    {
        var category = _categoryRepository.GetCategoryById(id);
        if (category == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }
        return Ok(category);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<Category>> Search([FromQuery] string? keyword)
    {
        var categories = _categoryRepository.GetCategories().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            categories = categories.Where(c =>
                c.CategoryName.ToLower().Contains(term) ||
                c.CategoryDesciption.ToLower().Contains(term));
        }
        return Ok(categories.ToList());
    }

    // =========================================================================================
    // [API TẠO DANH MỤC]: Tiếp nhận CategoryCreateUpdateDto và chuyển sang Repository
    // =========================================================================================
    [HttpPost]
    public IActionResult CreateCategory([FromBody] CategoryCreateUpdateDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.CategoryName) || string.IsNullOrWhiteSpace(dto.CategoryDesciption))
        {
            return BadRequest(new { message = "Category Name and Description are required." });
        }

        try
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName.Trim(),
                CategoryDesciption = dto.CategoryDesciption.Trim(),
                ParentCategoryId = dto.ParentCategoryId > 0 ? dto.ParentCategoryId : null,
                IsActive = dto.IsActive
            };

            var success = _categoryRepository.CreateCategory(category);
            if (success)
            {
                return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
            }

            return BadRequest(new { message = "Could not create category." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // =========================================================================================
    // [API CẬP NHẬT DANH MỤC]: Sửa tên, mô tả, danh mục cha hoặc trạng thái IsActive
    // =========================================================================================
    [HttpPut("{id}")]
    public IActionResult UpdateCategory(short id, [FromBody] CategoryCreateUpdateDto dto)
    {
        if (dto == null || id != dto.CategoryId)
        {
            return BadRequest(new { message = "ID in URL does not match request body." });
        }

        var existing = _categoryRepository.GetCategoryById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        try
        {
            var category = new Category
            {
                CategoryId = id,
                CategoryName = dto.CategoryName.Trim(),
                CategoryDesciption = dto.CategoryDesciption.Trim(),
                ParentCategoryId = dto.ParentCategoryId > 0 ? dto.ParentCategoryId : null,
                IsActive = dto.IsActive
            };

            var success = _categoryRepository.UpdateCategory(category);
            if (success)
            {
                return Ok(new { message = "Category updated successfully." });
            }

            return BadRequest(new { message = "Could not update category." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // =========================================================================================
    // [API XÓA DANH MỤC & RÀNG BUỘC NGHIỆP VỤ (TRANG 4 ĐỀ BÀI)]:
    // "The delete action will delete an item in case this item has not belonged to any news articles.
    // If the item is already stored in a news article cannot delete."
    // Tiếp theo: Gọi ICategoryRepository.DeleteCategory(id) -> CategoryDAO.DeleteCategory(id).
    // Nếu có bài viết tham chiếu -> Bắt InvalidOperationException và trả về 400 Bad Request.
    // =========================================================================================
    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(short id)
    {
        var existing = _categoryRepository.GetCategoryById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Category with ID {id} not found." });
        }

        try
        {
            var success = _categoryRepository.DeleteCategory(id);
            if (success)
            {
                return Ok(new { message = "Category deleted successfully." });
            }

            return BadRequest(new { message = "Could not delete category." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
