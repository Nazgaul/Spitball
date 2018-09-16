using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = Startup.PasswordRequiredLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}