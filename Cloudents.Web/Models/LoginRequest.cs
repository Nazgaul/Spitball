using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class LoginRequest
    {
        [EmailAddress(ErrorMessage = "EmailAddress"), Required(ErrorMessage = "Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

    }

    //public class EmailValidateRequest
    //{
    //    [EmailAddress(ErrorMessage = "EmailAddress"), Required(ErrorMessage = "Required")]
    //    public string Email { get; set; }
    //}
}
