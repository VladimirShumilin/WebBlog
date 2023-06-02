using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Request.Comment
{
    public class NewCommentRequest
    {
        [Required] // Указываем  параметр как обязательный
        Guid ArticleId { get; set; }
        [Required, MinLength(1, ErrorMessage = "Content is empty."), StringLength(200, ErrorMessage = "Content cannot exceed 200 characters.")]
        public string Content { get; set; } = "";
        public string Title { get; set; } = null!;
    }
}
