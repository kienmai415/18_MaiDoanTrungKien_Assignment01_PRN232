using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public interface ITagRepository
{
    List<Tag> GetTags();
    Tag? GetTagById(int tagId);
    bool CreateTag(Tag tag);
}
