
using System.ComponentModel.DataAnnotations;
using WebBlog.Contracts.Models.Request.Tag;

namespace WebBlog.Contracts.Models.Request.Article
{
    /// <summary>
    /// Запрос для добавления нового Тега 
    /// </summary>
    public class NewArticleRequest
    {
        [Required, MinLength(12, ErrorMessage = "AuthorId is empty")]
        public string AuthorId { get; set; } = default!;
        //Указываем параметр как обязательный с максимальныой длинной строки 100 символов
        [Required, StringLength(100, ErrorMessage = "Title name cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Content name cannot exceed 1000 characters.")]
        public string? Content { get; set; }

        public ICollection<TagRequest> Tags { get; set; } = default!;
    }
}
