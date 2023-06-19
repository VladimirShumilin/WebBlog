using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlog.DAL.Models;

namespace WebBlog.Contracts.Models.Responce.Article
{
    public class ArticleListViewModel
    {
        public List<ArticleViewModel> blogArticles { get; set; } = null!;

        //public ArticleListViewModel(IEnumerable<DAL.Models.Article> blogArticles, BlogUser user)
        //{
        //    _blogArticles = new List<ArticleViewModel>();
        //    foreach (var blogArticle in blogArticles)
        //    {
        //        _blogArticles.Add(new ArticleViewModel() blogArticle, user));
        //    }
        //}
    }
}
