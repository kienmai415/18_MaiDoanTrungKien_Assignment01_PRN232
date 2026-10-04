using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using _18_MaiDoanTrungKien_BackEnd.DTOs;
using _18_MaiDoanTrungKien_BackEnd.Models;
using _18_MaiDoanTrungKien_BackEnd.Repositories;

namespace _18_MaiDoanTrungKien_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsArticlesController : ControllerBase
{
    private readonly INewsArticleRepository _newsArticleRepository;

    public NewsArticlesController(INewsArticleRepository newsArticleRepository)
    {
        _newsArticleRepository = newsArticleRepository;
    }

    [HttpGet]
    [EnableQuery]
    public ActionResult<IEnumerable<NewsArticle>> GetNewsArticles()
    {
        var articles = _newsArticleRepository.GetNewsArticles();
        return Ok(articles);
    }

    [HttpGet("active")]
    [EnableQuery]
    public ActionResult<IEnumerable<NewsArticle>> GetActiveNewsArticles()
    {
        var activeArticles = _newsArticleRepository.GetActiveNewsArticles();
        return Ok(activeArticles);
    }

    [HttpGet("{id}")]
    public ActionResult<NewsArticle> GetNewsArticle(string id)
    {
        var article = _newsArticleRepository.GetNewsArticleById(id);
        if (article == null)
        {
            return NotFound(new { message = $"NewsArticle with ID '{id}' not found." });
        }
        return Ok(article);
    }

    [HttpGet("my-history/{staffId}")]
    public ActionResult<IEnumerable<NewsArticle>> GetMyNewsHistory(short staffId)
    {
        var articles = _newsArticleRepository.GetNewsArticlesByCreatedBy(staffId);
        return Ok(articles);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<NewsArticle>> Search([FromQuery] string? keyword, [FromQuery] bool? onlyActive)
    {
        var articles = onlyActive == true
            ? _newsArticleRepository.GetActiveNewsArticles().AsEnumerable()
            : _newsArticleRepository.GetNewsArticles().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            articles = articles.Where(a =>
                (a.NewsTitle != null && a.NewsTitle.ToLower().Contains(term)) ||
                (a.Headline != null && a.Headline.ToLower().Contains(term)) ||
                (a.NewsContent != null && a.NewsContent.ToLower().Contains(term)) ||
                (a.Category != null && a.Category.CategoryName.ToLower().Contains(term)) ||
                (a.Tags != null && a.Tags.Any(t => t.TagName.ToLower().Contains(term))));
        }

        return Ok(articles.ToList());
    }

    [HttpGet("reports")]
    public ActionResult<IEnumerable<NewsArticle>> GetReportStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest(new { message = "StartDate cannot be later than EndDate." });
        }

        var report = _newsArticleRepository.GetReportStatistics(startDate, endDate);
        return Ok(report);
    }

    [HttpPost]
    public IActionResult CreateNewsArticle([FromBody] NewsArticleCreateDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.NewsArticleId) || string.IsNullOrWhiteSpace(dto.NewsTitle) || string.IsNullOrWhiteSpace(dto.Headline))
        {
            return BadRequest(new { message = "Article ID, Title, and Headline are required." });
        }

        try
        {
            var article = new NewsArticle
            {
                NewsArticleId = dto.NewsArticleId.Trim(),
                NewsTitle = dto.NewsTitle.Trim(),
                Headline = dto.Headline.Trim(),
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId > 0 ? dto.CategoryId : null,
                NewsStatus = dto.NewsStatus,
                CreatedById = dto.CreatedById > 0 ? dto.CreatedById : null,
                CreatedDate = DateTime.Now
            };

            var success = _newsArticleRepository.CreateNewsArticle(article, dto.TagIds);
            if (success)
            {
                return CreatedAtAction(nameof(GetNewsArticle), new { id = article.NewsArticleId }, article);
            }

            return BadRequest(new { message = "Could not create news article." });
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

    [HttpPut("{id}")]
    public IActionResult UpdateNewsArticle(string id, [FromBody] NewsArticleUpdateDto dto)
    {
        if (dto == null || !string.Equals(id, dto.NewsArticleId, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "ID in URL does not match request body." });
        }

        var existing = _newsArticleRepository.GetNewsArticleById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"NewsArticle with ID '{id}' not found." });
        }

        try
        {
            var article = new NewsArticle
            {
                NewsArticleId = id,
                NewsTitle = dto.NewsTitle.Trim(),
                Headline = dto.Headline.Trim(),
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId > 0 ? dto.CategoryId : null,
                NewsStatus = dto.NewsStatus,
                UpdatedById = dto.UpdatedById > 0 ? dto.UpdatedById : null
            };

            var success = _newsArticleRepository.UpdateNewsArticle(article, dto.TagIds);
            if (success)
            {
                return Ok(new { message = "News article updated successfully." });
            }

            return BadRequest(new { message = "Could not update news article." });
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

    [HttpDelete("{id}")]
    public IActionResult DeleteNewsArticle(string id)
    {
        var existing = _newsArticleRepository.GetNewsArticleById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"NewsArticle with ID '{id}' not found." });
        }

        try
        {
            var success = _newsArticleRepository.DeleteNewsArticle(id);
            if (success)
            {
                return Ok(new { message = "News article deleted successfully." });
            }

            return BadRequest(new { message = "Could not delete news article." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
