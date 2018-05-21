using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RegisterEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[CaptchaValidator]
        public string Captcha { get; set; }
    }
}
