using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    [Table("Comments")]
    public record Comment
    {
        [Key]
        public Guid CommentId { get; set; }

        [ForeignKey("Article")]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string Content { get; set; } = "";
        public string Title { get; set; } = null!;
        [Required]
        public DateTime Created { get; set; }

        [Required]
        [Column(TypeName = "varchar(450)")]
        [ForeignKey("User")]
        public string UserID { get; set; } = null!;
        public BlogUser User { get; set; } = null!;
    }
}
