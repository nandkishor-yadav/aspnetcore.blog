using aspnetcore.blog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace aspnetcore.blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }

        public DbSet<About> Abouts { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }

        public DbSet<NewsLetter> NewsLetters { get; set; }
    }
}
