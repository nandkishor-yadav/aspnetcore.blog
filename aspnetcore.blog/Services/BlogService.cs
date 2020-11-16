using aspnetcore.blog.Data;
using aspnetcore.blog.Models;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore.blog.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetAllPostsAsync(int pageindex)
        {
            var posts = await _context.Posts.Where(e => e.IsPublished)
               .Select(p => new Post()
               {
                   Id = p.Id,
                   Title = p.Title,
                   Excerpt = p.Excerpt,
                   PubDate = p.PubDate,
                   CoverImagePath = p.CoverImagePath,
                   Tags = p.Tags,
                   IsPublished = p.IsPublished,
                   Slug = p.Slug
               })
               .OrderByDescending(e => e.Id).ToListAsync();

            var filteredPosts = PagingList.Create(posts, Constants.Blog.PostPerPage, pageindex);
           
            return filteredPosts;
        }

        public async Task<List<Post>> GetPostsByCategory(string category, int pageindex)
        {
            var posts = await _context.Posts
                .Where(p => p.Tags.Contains(category))
                .Select(p => new Post()
                {
                    Id = p.Id,
                    Title = p.Title,
                    PubDate = p.PubDate,
                    IsPublished = p.IsPublished,
                    Slug = p.Slug,
                    Tags = p.Tags,
                    Excerpt = p.Excerpt
                }).ToListAsync();
            var filteredPosts = PagingList.Create(posts, 50, pageindex);

            return filteredPosts;
        }

        public async Task<Post> GetPostByIdAsync(int Id)
        {
            return await _context.Posts.SingleOrDefaultAsync(x => x.Id == Id && x.IsPublished);
        }

        public async Task<bool> Contact(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
            var created = await _context.SaveChangesAsync();
            return created > 0;
        }

        public async Task<PrivacyPolicy> GetPrivacyByIdAsync(int Id)
        {
            return await _context.PrivacyPolicies.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<About> GetAboutIdAsync(int Id)
        {
            return await _context.Abouts.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> Subscribe(NewsLetter newsLetter)
        {
            await _context.NewsLetters.AddAsync(newsLetter);
            var created = await _context.SaveChangesAsync();
            return created > 0;
        }
    }
}
