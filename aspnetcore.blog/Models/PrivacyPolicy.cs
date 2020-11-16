using System.ComponentModel.DataAnnotations;

namespace aspnetcore.blog.Models
{
    public class PrivacyPolicy : Model
    {
        [Required]
        public string Content { get; set; }
    }
}
