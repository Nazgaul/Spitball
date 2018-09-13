using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace Cloudents.Web.Models
{
    public class LoginRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }


    public class RegisterRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required,Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    //public class SignUserRequest
    //{
    //    public SignUserRequest(string email, bool emailConfirmed, string name)
    //    {
    //        Email = email;
    //        EmailConfirmed = emailConfirmed;
    //        Name = name;
    //    }

    //    public SignUserRequest()
    //    {
    //    }

    //    [EmailAddress, Required]
    //    public string Email { get; set; }



    //    //[ModelBinder(typeof(ReturnUrlEntityBinder))]
    //    //[FromHeader]
    //    //public string ReturnUrl { get; set; }

    //    [BindNever]
    //    public bool EmailConfirmed { get; private set; }

    //    [BindNever]
    //    public string Name { get; private set; }
    //}
}
