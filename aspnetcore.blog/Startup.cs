using aspnetcore.blog.Controllers;
using aspnetcore.blog.Data;
using aspnetcore.blog.Extensions;
using aspnetcore.blog.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionIT.Mvc.Paging;
using System;
using System.IO.Compression;
using WebMarkupMin.AspNetCore3;
using WebMarkupMin.Core;
using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using WmmNullLogger = WebMarkupMin.Core.Loggers.NullLogger;
namespace aspnetcore.blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(
                     Environment.GetEnvironmentVariable("DefaultConnection")));

            services.AddControllersWithViews();

            services.AddScoped(x => new BlobServiceClient(Configuration.GetValue<string>("AzureBlobStorage")));
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserServices, UserServices>();

            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap4";
                options.PageParameterName = "p";
            });

            // Cookie authentication.
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    options =>
                    {
                        
                        options.LoginPath = "/login/";
                        options.LogoutPath = "/logout/";
                    });

            // HTML minification (https://github.com/Taritsyn/WebMarkupMin/wiki/ASP.NET-Core-3.1+)
            services
                .AddWebMarkupMin(
                    options =>
                    {
                        options.AllowMinificationInDevelopmentEnvironment = true;
                        options.DisablePoweredByHttpHeaders = true;
                    })
                .AddHtmlMinification(
                    options =>
                    {
                        options.MinificationSettings.RemoveOptionalEndTags = false;
                        options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
                    });
            services.AddSingleton<IWmmLogger, WmmNullLogger>(); // Used by HTML minifier

            // Bundling, minification and Sass transpilation (https://github.com/ligershark/WebOptimizer)
            services.AddWebOptimizer(
                pipeline =>
                {
                    pipeline.MinifyJsFiles();
                    pipeline.CompileScssFiles()
                            .InlineImages(1);
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //  Add IDistributedCache instance to dependency injection
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue<string>("RedisCache");
                options.InstanceName = "aspnetcore5blog/"; // For App Isolation - Prefix automatically added to any key written to the cache
            });

            services.AddProgressiveWebApp();

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseHttpsRedirection();
            app.UseResponseCompression();

            app.Use(
                (context, next) =>
                {
                    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                    return next();
                });

            
            app.UseWebOptimizer();
            app.UseStaticFilesWithCache();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseWebMarkupMin();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
