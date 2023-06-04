using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;

namespace WebBlog.Contracts.Models.Responce.Article
{
    public class ArticleViewModel
    {
        public Guid ArticleId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime Created { get; set; }

        public string AuthorId { get; set; } = null!;

        public virtual ICollection<CommentViewModel> Comments { get; set; } = null!;
        public virtual ICollection<TagViewModel> Tags { get; set; } = null!;
    }
}
