using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Request.Tag
{

    /// <summary>
    /// Запрос для обновления свойств Тега 
    /// </summary>
    public class EditTagRequest
    {
        [Required] // Указываем  параметр как обязательный
        public Guid TagId { get; set; }

        //Указываем параметр как обязательный с максимальныой длинной строки 20 символов
        [Required, MinLength(1, ErrorMessage = "Tag name is empty"), StringLength(20, ErrorMessage = "Тag name cannot exceed 20 characters.")]
        public string? Name { get; set; }
    }
}
