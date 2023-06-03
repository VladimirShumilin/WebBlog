using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebBlog.Contracts.Models.Request.Role
{
    public class NewRoleRequest
    {
        [Required(ErrorMessage = "Role Name is empty")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "SecurityLvl ")]
        public int SecurityLvl { get; set; }
    }
}
