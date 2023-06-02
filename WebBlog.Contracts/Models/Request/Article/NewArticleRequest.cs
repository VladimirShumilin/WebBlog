using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Request.Article
{
    /// <summary>
    /// Запрос для добавления нового Тега 
    /// </summary>
    public class NewArticleRequest
    {
        //Указываем параметр как обязательный с максимальныой длинной строки 100 символов
        [Required, StringLength(100, ErrorMessage = "Title name cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Content name cannot exceed 1000 characters.")]
        public string? Content { get; set; }

        public List<string> Tags { get; set; } = default!;
    }
}
