using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "StringLength", MinimumLength = Identity.SpitballIdentityExtensions.PasswordRequiredLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordNotMatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}