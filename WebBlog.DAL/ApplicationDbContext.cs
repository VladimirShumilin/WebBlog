using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBlog.DAL.Models;

namespace WebBlog.DAL
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Tag> Tag { get; set; } = default!;
        public DbSet<Article> Article { get; set; } = default!;
        public DbSet<Comment> Comment { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}