using aspnetcore.blog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aspnetcore.blog.Services
{
    public interface IBlogService
    {
        Task<List<Post>> GetAllPostsAsync(int pageindex);

        Task<Post> GetPostByIdAsync(int Id);

        Task<List<Post>> GetPostsByCategory(string category, int pageindex);

        Task<bool> Contact(Contact contact);

        Task<PrivacyPolicy> GetPrivacyByIdAsync(int Id);

        Task<About> GetAboutIdAsync(int Id);

        Task<bool> Subscribe(NewsLetter newsLetter);
    }
}
