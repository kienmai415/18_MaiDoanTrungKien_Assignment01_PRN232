using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _18_MaiDoanTrungKien_FrontEnd.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResultDto
{
    public short AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public int AccountRole { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

public class AccountViewModel
{
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70)]
    public string AccountEmail { get; set; } = string.Empty;

    public int AccountRole { get; set; } = 1; // 1: Staff, 2: Lecturer

    public string? AccountPassword { get; set; }

    public string RoleName => AccountRole switch
    {
        0 => "Admin",
        1 => "Staff",
        2 => "Lecturer",
        _ => "User"
    };
}

public class CategoryViewModel
{
    public short CategoryId { get; set; }

    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250)]
    public string CategoryDesciption { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    public bool IsActive { get; set; } = true;

    public CategoryViewModel? ParentCategory { get; set; }
}

public class TagViewModel
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class NewsArticleViewModel
{
    [Required(ErrorMessage = "Article ID is required.")]
    [StringLength(20)]
    public string NewsArticleId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(400)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Headline is required.")]
    [StringLength(150)]
    public string Headline { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; }

    public string? NewsContent { get; set; }

    [StringLength(400)]
    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public bool NewsStatus { get; set; } = true;

    public short? CreatedById { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public CategoryViewModel? Category { get; set; }

    public AccountViewModel? CreatedBy { get; set; }

    public AccountViewModel? UpdatedBy { get; set; }

    public List<TagViewModel> Tags { get; set; } = new List<TagViewModel>();

    public List<int> SelectedTagIds { get; set; } = new List<int>();
}
