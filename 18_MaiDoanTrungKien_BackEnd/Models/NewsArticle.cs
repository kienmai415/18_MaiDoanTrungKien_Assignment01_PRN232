using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _18_MaiDoanTrungKien_BackEnd.Models;

[Table("NewsArticle")]
public partial class NewsArticle
{
    [Key]
    [Column("NewsArticleID")]
    [StringLength(20)]
    public string NewsArticleId { get; set; } = null!;

    [StringLength(400)]
    public string NewsTitle { get; set; } = null!;

    [StringLength(150)]
    public string Headline { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public string? NewsContent { get; set; }

    [StringLength(400)]
    public string? NewsSource { get; set; }

    [Column("CategoryID")]
    public short? CategoryId { get; set; }

    public bool NewsStatus { get; set; }

    [Column("CreatedByID")]
    public short? CreatedById { get; set; }

    [Column("UpdatedByID")]
    public short? UpdatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("NewsArticles")]
    public virtual Category? Category { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("NewsArticleCreatedBies")]
    public virtual SystemAccount? CreatedBy { get; set; }

    [ForeignKey("UpdatedById")]
    [InverseProperty("NewsArticleUpdatedBies")]
    public virtual SystemAccount? UpdatedBy { get; set; }

    [ForeignKey("NewsArticleId")]
    [InverseProperty("NewsArticles")]
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
