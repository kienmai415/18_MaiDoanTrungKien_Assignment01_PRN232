using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _18_MaiDoanTrungKien_BackEnd.Models;

[Table("SystemAccount")]
[Index("AccountEmail", Name = "UQ__SystemAc__FC770D33C98ACDFB", IsUnique = true)]
public partial class SystemAccount
{
    [Key]
    [Column("AccountID")]
    public short AccountId { get; set; }

    [StringLength(100)]
    public string? AccountName { get; set; }

    [StringLength(70)]
    public string AccountEmail { get; set; } = null!;

    public int? AccountRole { get; set; }

    [StringLength(70)]
    public string AccountPassword { get; set; } = null!;

    [InverseProperty("CreatedBy")]
    public virtual ICollection<NewsArticle> NewsArticleCreatedBies { get; set; } = new List<NewsArticle>();

    [InverseProperty("UpdatedBy")]
    public virtual ICollection<NewsArticle> NewsArticleUpdatedBies { get; set; } = new List<NewsArticle>();
}
