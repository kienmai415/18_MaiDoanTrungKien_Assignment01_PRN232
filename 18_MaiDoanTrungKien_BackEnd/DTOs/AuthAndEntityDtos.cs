using System;
using System.Collections.Generic;

namespace _18_MaiDoanTrungKien_BackEnd.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public short AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public int AccountRole { get; set; } // 0: Admin, 1: Staff, 2: Lecturer
    public string RoleName { get; set; } = string.Empty;
}

public class AccountCreateDto
{
    public string AccountName { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public int AccountRole { get; set; } = 1;
    public string AccountPassword { get; set; } = string.Empty;
}

public class AccountUpdateDto
{
    public short AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public int AccountRole { get; set; } = 1;
    public string? AccountPassword { get; set; }
}

public class CategoryCreateUpdateDto
{
    public short CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDesciption { get; set; } = string.Empty;
    public short? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class NewsArticleCreateDto
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string NewsTitle { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public bool NewsStatus { get; set; } = true;
    public short? CreatedById { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
}

public class NewsArticleUpdateDto
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string NewsTitle { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string? NewsContent { get; set; }
    public string? NewsSource { get; set; }
    public short? CategoryId { get; set; }
    public bool NewsStatus { get; set; } = true;
    public short? UpdatedById { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
}
