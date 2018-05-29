using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PhoneNumberRequest
    {
        [Required]
        public string Number { get; set; }
    }

    public class CodeRequest
    {
        [Required]
        public string Number { get; set; }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
