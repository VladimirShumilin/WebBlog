using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Request.Article
{

    /// <summary>
    /// Запрос для обновления свойств Тега 
    /// </summary>
    public class EditArticleRequest
    {
        [Required] // Указываем  параметр как обязательный
        public Guid ArticleId { get; set; }

        //Указываем параметр как обязательный с максимальныой длинной строки 100 символов
        [Required, StringLength(100, ErrorMessage = "Тag name cannot exceed 100 characters.")]
        public string? Name { get; set; }
    }
}
