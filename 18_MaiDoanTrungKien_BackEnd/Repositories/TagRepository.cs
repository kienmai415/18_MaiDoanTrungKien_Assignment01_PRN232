using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public class TagRepository : ITagRepository
{
    public List<Tag> GetTags() => TagDAO.Instance.GetTags();

    public Tag? GetTagById(int tagId) => TagDAO.Instance.GetTagById(tagId);

    public bool CreateTag(Tag tag) => TagDAO.Instance.CreateTag(tag);
}
