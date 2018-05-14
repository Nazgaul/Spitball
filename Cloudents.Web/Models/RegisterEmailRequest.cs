using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RegisterEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Captcha { get; set; }
    }
}
