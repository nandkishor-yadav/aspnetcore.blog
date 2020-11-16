using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnetcore.blog.Models
{
    public class Contact : Model
    {
        [Required]
        [DisplayName("Name")]
        public string Firstname { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
