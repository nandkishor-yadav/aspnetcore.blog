using System.ComponentModel.DataAnnotations;

namespace aspnetcore.blog.Models
{
    public class About : Model
    {
        [Required]
        public string Content { get; set; }
    }
}
