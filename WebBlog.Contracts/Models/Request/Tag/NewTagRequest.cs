using System.ComponentModel.DataAnnotations;

namespace WebBlog.Contracts.Models.Request.Tag
{
    /// <summary>
    /// Запрос для добавления нового Тега 
    /// </summary>
    public class NewTagRequest
    {
        //Указываем параметр как обязательный с максимальныой длинной строки 20 символов
        [Required, MinLength(1, ErrorMessage = "Tag name is empty"), StringLength(20, ErrorMessage = "Тag name cannot exceed 20 characters.")]
        public string? Name { get; set; }
    }
}
