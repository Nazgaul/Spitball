using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class ForgotPasswordRequest
    {
        [EmailAddress, Required]
        public string Email { get; set; }
    }
}