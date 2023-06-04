using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Query.User
{
    /// <summary>
    /// Представляет сущность BlogUser для отображения в формах 
    /// </summary>
    public class UserViewModel
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public string CustomField { get; set; } = null!;
        public List<string> UserRoles { get; set; } = null!;
    }
}
