using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBlog.Contracts.Models.Responce.Tag
{
    public class TagViewModel
    {
        public Guid TagId { get; set; }
        public string Name { get; set; } = "";
    }
}
