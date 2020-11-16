using aspnetcore.blog.Extensions;
using aspnetcore.blog.Models;
using aspnetcore.blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace aspnetcore.blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService _blogService;
        private readonly IConfiguration _config;
        readonly EmailSender emailSender = new EmailSender();
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, IBlogService blogService, IConfiguration configuration, IDistributedCache cache)
        {
            _logger = logger;
            _blogService = blogService;
            _config = configuration;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int p = 1)
        {
            var model = await _blogService.GetAllPostsAsync(p); 
            return View(model);
        }

        [Route("/category/{category}")]
        public async Task<IActionResult> Category(string category, int pageindex)
        {
            var categories = await _blogService.GetPostsByCategory(category, pageindex);
            return View("~/Views/Home/Index.cshtml", categories);
        }

        
        [HttpGet("/article/{id}/{slug}", Name = "Postpage")]
        public async Task<IActionResult> Postpage(int Id)
        {
            if(Constants.Blog.UseRedisCache == "y")
            {
                var post = await _cache.GetRecordAsync<Post>(Id.ToString());

                if (post == null)
                {
                    post = await _blogService.GetPostByIdAsync(Id);

                    await _cache.SetRecordAsync(Id.ToString(), post);
                }
                return View(post);
            }
            else
            {
                var post = await _blogService.GetPostByIdAsync(Id);
                return View(post);
            }
        }

        public async Task<IActionResult> Privacy()
        {
            var privacy = await _blogService.GetPrivacyByIdAsync(1);

            return View(privacy);
        }

        public async Task<IActionResult> About()
        {
            var about = await _blogService.GetAboutIdAsync(1);

            return View(about);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Contact(Contact contact)
        {

            if (ModelState.IsValid)
            {
                if (!await GoogleRecaptchaHelper.IsReCaptchaPassedAsync(Request.Form["g-recaptcha-response"],
                    _config["GoogleReCaptcha:secret"]))
                {
                    ModelState.AddModelError(string.Empty, "You failed the CAPTCHA");
                    return View(contact);
                }

                await _blogService.Contact(contact);

                await emailSender.Execute(contact.Email + " sent you message", contact.Message, "");
                TempData["msg"] = "Thank you for your message, We will read it and get in touch with you if needed";

            }

            return View();
        }

        #region GoogleRecaptchaHelper
        public static class GoogleRecaptchaHelper
        {
            // A function that checks reCAPTCHA results
            public static async Task<bool> IsReCaptchaPassedAsync(string gRecaptchaResponse, string secret)
            {
                HttpClient httpClient = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", gRecaptchaResponse)
            });
                var res = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                string JSONres = res.Content.ReadAsStringAsync().Result;
                dynamic JSONdata = JObject.Parse(JSONres);
                if (JSONdata.success != "true")
                {
                    return false;
                }

                return true;
            }
        }
        #endregion

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404 || statusCode.Value == 500)
                {
                    var viewName = statusCode.ToString();
                    return View(viewName);
                }
            }
            return View();
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsLetter(NewsLetter newsLetter)
        {
            if (string.IsNullOrEmpty(newsLetter.EmailAddress))
            {
                return Json(new { success = false, responseText = "Email address is required" });
            }

            if (!IsValid(newsLetter.EmailAddress))
            {
                return Json(new { success = false, responseText = "The email address is not a valid e-mail address" });
            }

            await _blogService.Subscribe(newsLetter);

            return Json(new { success = true, responseText = "Thank you for subscribing" });
        }
    }
}
