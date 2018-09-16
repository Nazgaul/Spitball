using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RegisterRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required,Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}