using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class LoginRequest
    {
        [EmailAddress,Required]
        public string Email { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Captcha { get; set; }
    }
}
