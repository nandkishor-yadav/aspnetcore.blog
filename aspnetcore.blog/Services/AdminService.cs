using aspnetcore.blog.Controllers;
using aspnetcore.blog.Data;
using aspnetcore.blog.Extensions;
using aspnetcore.blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PhotoSauce.MagicScaler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace aspnetcore.blog.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;
        private readonly IWebHostEnvironment env;
        private const string FILES = "files";
        private const string POSTS = "Posts";
        private readonly string folder;

        public AdminService(ApplicationDbContext context, IBlobService blobService, IWebHostEnvironment env)
        {
            _context = context;
            _blobService = blobService;
            this.env = env;
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            this.folder = Path.Combine(env.WebRootPath, POSTS);            
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            var posts = await _context.Posts
           .Select(t => new Post()
            {
                Id = t.Id,
                Title = t.Title,
                PubDate = t.PubDate,
                IsPublished = t.IsPublished,
                Slug = t.Slug
            })
                .OrderByDescending(o => o.Id).ToListAsync();

            return posts;
        }

        public async Task<Post> GetPostsByIdAsync(int Id)
        {
            return await _context.Posts.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Post> CreatePostAsync(Post model)
        {
            string CoverImagePath;
            if (Constants.Blog.UseBlobStorage == "y")
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverImage.FileName;
                await _blobService.UploadFileBlobAsync(Constants.Blog.BlobContainerNameFeaturedImage, model.CoverImage.OpenReadStream(), model.CoverImage.ContentType, uniqueFileName);
                CoverImagePath = $"/{Constants.Blog.BlobContainerNameFeaturedImage}/{uniqueFileName}";
            }
            else
            {
                CoverImagePath = await UploadedFile(model);
            }

            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(model.Title);

            await SaveFilesToDisk(model).ConfigureAwait(false);

            Post post = new Post
            {
                Title = model.Title,
                Excerpt = model.Excerpt,
                Content = model.Content,
                IsPublished = model.IsPublished,
                Tags = model.Tags,
                CoverImagePath = CoverImagePath,
                PubDate = DateTime.Now,
                Slug = friendlyTitle
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> UpdatePostAsync(Post model)
        {
            string CoverImagePath = string.Empty;

            if (Constants.Blog.UseBlobStorage == "y")
            {
                if (model.CoverImage != null)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverImage.FileName;
                    await _blobService.UploadFileBlobAsync(Constants.Blog.BlobContainerNameFeaturedImage, model.CoverImage.OpenReadStream(), model.CoverImage.ContentType, uniqueFileName);
                    CoverImagePath = $"/{Constants.Blog.BlobContainerNameFeaturedImage}/{uniqueFileName}";
                }
                else
                {
                    CoverImagePath = model.ExistingCoverImagePath;
                }
            }
            else if(Constants.Blog.UseBlobStorage == "n")
            {
                if (model.CoverImage != null)
                {
                    CoverImagePath = await UploadedFile(model);
                }
                else
                {
                    CoverImagePath = model.ExistingCoverImagePath;
                }
            }

            await SaveFilesToDisk(model).ConfigureAwait(false);

            Post editPost = new Post
            {
                Id = model.Id,
                Title = model.Title,
                Excerpt = model.Excerpt,
                Content = model.Content,
                IsPublished = model.IsPublished,
                Tags = model.Tags,
                CoverImagePath = CoverImagePath,
                LastModified = DateTime.Now,
                PubDate = model.PubDate,
                Slug = model.Slug
            };

            _context.Posts.Update(editPost);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(int Id)
        {
            var post = await GetPostsByIdAsync(Id);
            if (post == null)
            {
                return false;
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PrivacyPolicy>> GetPrivacyPolicies()
        {
            var policies = await _context.PrivacyPolicies
          .Select(t => new PrivacyPolicy()
          {
              Id = t.Id,
              Content = t.Content,
              PubDate = t.PubDate,
              LastModified = t.LastModified,
              IsPublished = t.IsPublished
          })
               .OrderByDescending(o => o.Id).ToListAsync();

            return policies;
        }
        public async Task<bool> UpdatePrivacyPolicyAsync(PrivacyPolicy model)
        {
            if (model.Id == 0)
            {
                PrivacyPolicy privacy = new PrivacyPolicy
                {
                    Content = model.Content,
                    PubDate = DateTime.Now,
                    IsPublished = model.IsPublished
                };
                await _context.PrivacyPolicies.AddAsync(privacy);
            }
            else
            {
                PrivacyPolicy privacy = new PrivacyPolicy
                {
                    Id = model.Id,
                    Content = model.Content,
                    IsPublished = model.IsPublished,
                    LastModified = DateTime.Now
                };
                _context.PrivacyPolicies.Update(privacy);
            }

            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<List<About>> GetAboutSection()
        {
            var about = await _context.Abouts
          .Select(t => new About()
          {
              Id = t.Id,
              Content = t.Content,
              PubDate = t.PubDate,
              LastModified = t.LastModified,
              IsPublished = t.IsPublished
          })
               .OrderByDescending(o => o.Id).ToListAsync();

            return about;
        }
        public async Task<bool> UpdateAboutSectionAsync(About model)
        {
            if (model.Id == 0)
            {
                About about = new About
                {
                    Content = model.Content,
                    PubDate = DateTime.Now,
                    IsPublished = model.IsPublished
                };
                await _context.Abouts.AddAsync(about);
            }
            else
            {
                About about = new About
                {
                    Id = model.Id,
                    Content = model.Content,
                    IsPublished = model.IsPublished,
                    LastModified = DateTime.Now
                };
                _context.Abouts.Update(about);
            }

            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        #region UploadedFile
        private async Task<string> UploadedFile(Post post)
        {
            string uniqueFileName = null;

            if (post.CoverImage != null)
            {

                string uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + post.CoverImage.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                MagicImageProcessor.ProcessImage(post.CoverImage.OpenReadStream(), fileStream, ImageOptions());
                await post.CoverImage.CopyToAsync(fileStream);
            }
            return uniqueFileName;
        }

        private ProcessImageSettings ImageOptions() => new ProcessImageSettings
        {
            Width = 750,
            Height = 375,
            ResizeMode = CropScaleMode.Crop,
            SaveFormat = FileFormat.Jpeg,
            JpegQuality = 100,
            JpegSubsampleMode = ChromaSubsampleMode.Subsample420
        };
        #endregion

        #region SavePostImagesToAzure
        public async Task SaveFilesToDisk(Post post)
        {
            var imgRegex = new Regex("<img[^>]+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var base64Regex = new Regex("data:[^/]+/(?<ext>[a-z]+);base64,(?<base64>.+)", RegexOptions.IgnoreCase);
            var allowedExtensions = new[] {
              ".jpg",
              ".jpeg",
              ".gif",
              ".png",
              ".webp"
            };

            foreach (Match? match in imgRegex.Matches(post.Content))
            {
                if (match is null)
                {
                    continue;
                }

                var doc = new XmlDocument();
                doc.LoadXml($"<root>{match.Value}</root>");

                var img = doc.FirstChild.FirstChild;
                var srcNode = img.Attributes["src"];
                var fileNameNode = img.Attributes["data-filename"];

                // The HTML editor creates base64 DataURIs which we'll have to convert to image
                // files on disk
                if (srcNode is null || fileNameNode is null)
                {
                    continue;
                }

                var extension = Path.GetExtension(fileNameNode.Value);

                // Only accept image files
                if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var base64Match = base64Regex.Match(srcNode.Value);
                if (base64Match.Success)
                {
                    var bytes = Convert.FromBase64String(base64Match.Groups["base64"].Value);
                    srcNode.Value = await SaveFile(bytes, fileNameNode.Value).ConfigureAwait(false);

                    img.Attributes.Remove(fileNameNode);
                    post.Content = post.Content.Replace(match.Value, img.OuterXml, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public async Task<string> SaveFile(byte[] bytes, string fileName, string? suffix = null)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            suffix = CleanFromInvalidChars(suffix ?? DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));

            var ext = Path.GetExtension(fileName);
            var name = CleanFromInvalidChars(Path.GetFileNameWithoutExtension(fileName));

            var fileNameWithSuffix = $"{name}_{suffix}{ext}";

            var absolute = Path.Combine(this.folder, FILES, fileNameWithSuffix);
            var dir = Path.GetDirectoryName(absolute);

            if (Constants.Blog.UseBlobStorage == "y")
            {
                string ContentType = "image/png";
                MemoryStream stream = new MemoryStream(bytes);
                var result = await _blobService.UploadFileBlobAsync(Constants.Blog.BlobContainerName, stream, ContentType, fileNameWithSuffix);
                var PhotoPath = result.LocalPath;
                return PhotoPath;
            }
            else
            {
                Directory.CreateDirectory(dir);
                using (var writer = new FileStream(absolute, FileMode.CreateNew))
                {
                    await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                }

                return $"/{POSTS}/{FILES}/{fileNameWithSuffix}";
            }
        }


        private static string CleanFromInvalidChars(string input)
        {
            var regexSearch = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()));
            var r = new Regex($"[{regexSearch}]");
            return r.Replace(input, string.Empty);
        }

        

        #endregion
    }
}
