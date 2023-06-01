using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    [Table("Articles")]
    public record Article
    {
        [Key]
        public Guid ArticleId { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Title { get; set; } = null!;
        [Required]
        [Column(TypeName = "varchar(1000)")]
        public string Content { get; set; } = null!;
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }

        [Required]
        [Column(TypeName = "varchar(450)")]
        [ForeignKey("User")]
        public string AuthorId { get; set; } = null!;
        public BlogUser User { get; set; } = null!; 

        public virtual ICollection<Comment> Comments { get; set; } = null!;


        public virtual ICollection<Tag> Tags { get; set; } = null!;

       

    }
}
