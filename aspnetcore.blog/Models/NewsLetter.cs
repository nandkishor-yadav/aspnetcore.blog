using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnetcore.blog.Models
{
    public class NewsLetter : Model
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        [DisplayName("Email")]
        public string EmailAddress { get; set; }
    }
}
