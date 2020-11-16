using aspnetcore.blog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aspnetcore.blog.Services
{
    public interface IAdminService
    {
        Task<List<Post>> GetAllPostsAsync();

        Task<Post> GetPostsByIdAsync(int Id);

        Task<Post> CreatePostAsync(Post model);

        Task<bool> UpdatePostAsync(Post model);

        Task<bool> DeletePostAsync(int Id);

        Task<List<PrivacyPolicy>> GetPrivacyPolicies();

        Task<bool> UpdatePrivacyPolicyAsync(PrivacyPolicy model);

        Task<List<About>> GetAboutSection();

        Task<bool> UpdateAboutSectionAsync(About model);
    }
}
