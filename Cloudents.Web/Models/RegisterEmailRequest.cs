using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Binders;
using Cloudents.Web.Filters;

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
