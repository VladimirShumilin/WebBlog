using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.DAL.Models
{
    public class BlogUser : IdentityUser
    {
        // Дополнительные свойства пользователя
        [Comment("Дополнительный атрибут пользователя")]
        [Column(TypeName = "varchar(100)")]
        public string? CustomField { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = null!;
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = null!;
    }
}
