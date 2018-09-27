using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ForgotPasswordRequest
    {
        [EmailAddress(ErrorMessage = "EmailAddress"), Required(ErrorMessage = "Required")]
        public string Email { get; set; }
    }
}