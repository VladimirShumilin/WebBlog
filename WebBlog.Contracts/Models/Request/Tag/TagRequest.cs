using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Request.Tag
{
    public class TagRequest
    {
        public Guid TagId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = default!;
    }
}
