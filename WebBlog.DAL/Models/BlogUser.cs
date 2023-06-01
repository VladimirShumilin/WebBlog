using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.DAL.Models
{
    public class BlogUser : IdentityUser
    {
        // Дополнительные свойства пользователя
        public string? CustomField { get; set; }
    }
}
