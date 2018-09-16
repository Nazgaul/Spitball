using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class RegisterEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
