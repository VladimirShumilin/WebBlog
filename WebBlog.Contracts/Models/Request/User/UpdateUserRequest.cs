using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.Contracts.Models.Request.User
{
    public  class UpdateUserRequest
    {

        [StringLength(100, ErrorMessage = "CustomField cannot exceed 100 characters.")]
        public string CustomField { get; set; } = null!;
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

    }
}
