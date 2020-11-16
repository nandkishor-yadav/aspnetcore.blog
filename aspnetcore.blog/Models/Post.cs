using aspnetcore.blog.Extensions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;

namespace aspnetcore.blog.Models
{
    public class Post : Model
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Tags { get; set; }

        public string CoverImagePath { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".webp" })]
        public IFormFile CoverImage { get; set; }

        [NotMapped]
        public string ExistingCoverImagePath { get; set; }

        public string RenderContent()
        {
            var result = this.Content;

            // Set up lazy loading of images/iframes
            if (!string.IsNullOrEmpty(result))
            {
                // Set up lazy loading of images/iframes
                var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
                var pattern = "(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)";
                result = Regex.Replace(result, pattern, m => m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);

                // Youtube content embedded using this syntax: [youtube:xyzAbc123]
                var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
                result = Regex.Replace(
                    result,
                    @"\[youtube:(.*?)\]",
                    m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));
            }

            return result;
        }
    }
}
