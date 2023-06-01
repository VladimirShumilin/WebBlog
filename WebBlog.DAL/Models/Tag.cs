using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.DAL.Models
{
    [Table("Tags")]
    public record Tag
    {
        [Key]
        public Guid TagId { get; set; }
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Name { get; set; } = "";

        public virtual ICollection<Article> Articles { get; } = null!;



    }
}
