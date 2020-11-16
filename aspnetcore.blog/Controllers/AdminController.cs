using aspnetcore.blog.Extensions;
using aspnetcore.blog.Models;
using aspnetcore.blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace aspnetcore.blog.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IBlogService _blogService;

        public AdminController(IAdminService adminService, IBlogService blogService)
        {
            _adminService = adminService;
            _blogService = blogService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _adminService.GetAllPostsAsync();

            return View(model);
        }

        #region NewPost
        public IActionResult NewPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPost(Post post)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                await _adminService.CreatePostAsync(post);

                return RedirectToAction("Index", "Admin").Success_alert("Post created ", "successfully");
            }
            catch (Exception ex)
            {

                return RedirectToAction("NewPost", "Admin").Danger_alert("Error", "Something went wrong. Please try again in few moments or contact administrator" + ex);
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var post = await _adminService.GetPostsByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            Post model = new Post
            {
                Id = post.Id,
                PubDate = post.PubDate,
                LastModified = post.LastModified,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Content = post.Content,
                ExistingCoverImagePath = post.CoverImagePath,
                IsPublished = post.IsPublished,
                Tags = post.Tags,
                Slug = post.Slug
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            await _adminService.UpdatePostAsync(post);

            return RedirectToAction("Index", "Admin").Success_alert("Post updated", " successfully");
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            await _adminService.DeletePostAsync(id);

            TempData["msg"] = "Deleted succefully !!";
            return Json("success");
        }
        #endregion

        #region PrivacyPolicy
        public async Task<IActionResult> PrivacyPolicy()
        {
            var policies = await _adminService.GetPrivacyPolicies();
            
            return View(policies);
        }
        public IActionResult NewPrivacyPolicy()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> UpdatePrivacyPolicy(int id)
        {
            var privacyPolicy = await _blogService.GetPrivacyByIdAsync(id);

            if (privacyPolicy is null)
            {
                return NotFound();
            }

            PrivacyPolicy privacy = new PrivacyPolicy
            {
                Id = privacyPolicy.Id,
                Content = privacyPolicy.Content,
                IsPublished = privacyPolicy.IsPublished
            };
            return View(privacy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacyPolicy(PrivacyPolicy privacyPolicy)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                await _adminService.UpdatePrivacyPolicyAsync(privacyPolicy);

                return RedirectToAction("PrivacyPolicy", "Admin").Success_alert("Success", "");
            }
            catch (Exception)
            {

                return RedirectToAction("UpdatePrivacyPolicy", "Admin").Danger_alert("Error", "Something went wrong. Please try again in few moments or contact administrator");
            }
        }
        #endregion

        #region AboutSection
        public async Task<IActionResult> About()
        {
            var about = await _adminService.GetAboutSection();

            return View(about);
        }
        public IActionResult NewAbout()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> UpdateAbout(int id)
        {
            var about = await _blogService.GetAboutIdAsync(id);

            if (about is null)
            {
                return NotFound();
            }

            About abt = new About
            {
                Id = about.Id,
                Content = about.Content,
                IsPublished = about.IsPublished
            };
            return View(abt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAbout(About about)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                await _adminService.UpdateAboutSectionAsync(about);

                return RedirectToAction("About", "Admin").Success_alert("Success", "");
            }
            catch (Exception)
            {

                return RedirectToAction("About", "Admin").Danger_alert("Error", "Something went wrong. Please try again in few moments or contact administrator");
            }
        }
        #endregion
    }
}
