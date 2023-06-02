using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Query.User
{
    /// <summary>
    /// Представляет сущность BlogUser для отображения в формах Добавить и Редактировать
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; } = "";
        //Указываем параметр как обязательный с максимальныой длинной строки 20 символов
        [Required, StringLength(20, ErrorMessage = "Email cannot exceed 20 characters.")]
        public string? Email { get; set; }


        [Required, MinLength(1, ErrorMessage = "UserName is empty"), StringLength(20, ErrorMessage = "UserName cannot exceed 20 characters.")]
        public string UserName { get; set; } = "";

        [Required, MinLength(1, ErrorMessage = "Password is empty"), StringLength(20, ErrorMessage = "Password cannot exceed 20 characters.")]
        public string Password { get; set; } = "";

    }
}
