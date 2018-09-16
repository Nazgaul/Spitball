using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class LoginRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
