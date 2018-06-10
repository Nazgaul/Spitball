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
        [RegularExpression(@"^\+?[1-9]\d{1,14}$")]
        public string Number { get; set; }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
