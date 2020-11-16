using aspnetcore.blog.Models;
using aspnetcore.blog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace aspnetcore.blog.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserServices _userservice;

        public AccountController(IUserServices userservice)
        {
            _userservice = userservice;
        }

        [Route("/login")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            this.ViewData[Constants.Blog.ReturnUrl] = returnUrl;
            return this.View();
        }

        [Route("/login")]
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(string? returnUrl, LoginViewModel? model)
        {
            this.ViewData[Constants.Blog.ReturnUrl] = returnUrl;

            if (model is null || model.UserName is null || model.Password is null)
            {
                this.ModelState.AddModelError(string.Empty, "User name/password is invalid");
                return this.View(nameof(Login), model);
            }

            if (!this.ModelState.IsValid || !_userservice.ValidateUser(model.UserName, model.Password))
            {
                this.ModelState.AddModelError(string.Empty, "User name/password is invalid");
                return this.View(nameof(Login), model);
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));

            var principle = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties { IsPersistent = model.RememberMe };
            await this.HttpContext.SignInAsync(principle, properties).ConfigureAwait(false);

            return this.LocalRedirect(returnUrl ?? "/");
        }

        [Route("/logout")]
        public async Task<IActionResult> LogOutAsync()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return this.LocalRedirect("/");
        }
    }
}
