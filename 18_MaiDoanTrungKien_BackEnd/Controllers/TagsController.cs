using Microsoft.AspNetCore.Mvc;
using _18_MaiDoanTrungKien_BackEnd.Models;
using _18_MaiDoanTrungKien_BackEnd.Repositories;

namespace _18_MaiDoanTrungKien_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;

    public TagsController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Tag>> GetTags()
    {
        return Ok(_tagRepository.GetTags());
    }

    [HttpGet("{id}")]
    public ActionResult<Tag> GetTag(int id)
    {
        var tag = _tagRepository.GetTagById(id);
        if (tag == null)
        {
            return NotFound(new { message = $"Tag with ID {id} not found." });
        }
        return Ok(tag);
    }

    [HttpPost]
    public IActionResult CreateTag([FromBody] Tag tag)
    {
        if (tag == null || string.IsNullOrWhiteSpace(tag.TagName))
        {
            return BadRequest(new { message = "Tag name is required." });
        }

        var success = _tagRepository.CreateTag(tag);
        if (success)
        {
            return CreatedAtAction(nameof(GetTag), new { id = tag.TagId }, tag);
        }

        return BadRequest(new { message = "Could not create tag." });
    }
}
