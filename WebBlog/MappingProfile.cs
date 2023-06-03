using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebBlog.Contracts.Models.Query.User;
using WebBlog.Contracts.Models.Request.Article;
using WebBlog.Contracts.Models.Request.Comment;
using WebBlog.Contracts.Models.Request.Role;
using WebBlog.Contracts.Models.Request.Tag;
using WebBlog.Contracts.Models.Responce.Article;
using WebBlog.Contracts.Models.Responce.Comment;
using WebBlog.Contracts.Models.Responce.Tag;
using WebBlog.DAL.Models;

namespace WebBlog
{
    /// <summary>
    /// Настройки маппинга всех сущностей приложения
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// В конструкторе настроим соответствие сущностей при маппинге
        /// </summary>
        public MappingProfile()
        {
            CreateMap<EditArticleRequest,Article >();
            CreateMap<NewArticleRequest, Article>();

            CreateMap<UserViewModel, BlogUser>();
            CreateMap<BlogUser, UserViewModel>();
        
            CreateMap<CommentViewModel, Comment>();
            CreateMap<TagViewModel,Tag >();

            CreateMap<NewCommentRequest, Comment>();
            CreateMap<EditCommentRequest, Comment>();

            CreateMap<TagRequest, Tag>();

            CreateMap<NewUserRequest,BlogUser>();
            CreateMap<Comment, CommentViewModel>();
            CreateMap<Tag, TagViewModel>();
            CreateMap<Article, ArticleViewModel>();

            CreateMap<EditTagRequest, Tag>();
            CreateMap<NewTagRequest, Tag>();


            CreateMap<NewRoleRequest, IdentityRole>();
            CreateMap<EditRoleRequest, IdentityRole>();
            
        }
    }
}
