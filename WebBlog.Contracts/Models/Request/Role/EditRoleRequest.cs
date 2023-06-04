using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Request.Role
{
    public class EditRoleRequest
    {
        [Required]
        public string Id { get; set; } = default!;
        [Required(ErrorMessage = "Role Name is empty")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "SecurityLvl ")]
        public int SecurityLvl { get; set; }
    }
}
