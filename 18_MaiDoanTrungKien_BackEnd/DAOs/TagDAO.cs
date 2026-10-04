using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.DAOs;

public class TagDAO
{
    private static TagDAO? _instance;
    private static readonly object _instanceLock = new object();

    private TagDAO() { }

    public static TagDAO Instance
    {
        get
        {
            lock (_instanceLock)
            {
                _instance ??= new TagDAO();
                return _instance;
            }
        }
    }

    public List<Tag> GetTags()
    {
        using var context = new FunewsManagementContext();
        return context.Tags.AsNoTracking().ToList();
    }

    public Tag? GetTagById(int tagId)
    {
        using var context = new FunewsManagementContext();
        return context.Tags.AsNoTracking().FirstOrDefault(t => t.TagId == tagId);
    }

    public bool CreateTag(Tag tag)
    {
        using var context = new FunewsManagementContext();
        context.Tags.Add(tag);
        return context.SaveChanges() > 0;
    }
}
