using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebBlog.DAL.Models
{
    /// <summary>
    /// Класс отнощений многие ко многим между таблицами Articles и Tags
    /// </summary>
    [PrimaryKey(nameof(ArticleId), nameof(TagId))]
    public record ArticleTag
    {
        [Required]
        public Guid ArticleId { get; set; }
        [Required]
        public Guid TagId { get; set; }
        public Article Post { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
