using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(100,MinimumLength = 2,ErrorMessage = "StringLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "EmailAddress"), Required(ErrorMessage = "Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "StringLength", MinimumLength = Identity.SpitballIdentityExtensions.PasswordRequiredLength)]

        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [Compare("Password", ErrorMessage = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}