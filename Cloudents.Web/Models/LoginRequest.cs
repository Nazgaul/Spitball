using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class LoginRequest
    {
        [EmailAddress,Required]
        public string Email { get; set; }

        public bool RememberMe { get; set; }

        //[Required]
        //public string Key { get; set; }
    }
}
