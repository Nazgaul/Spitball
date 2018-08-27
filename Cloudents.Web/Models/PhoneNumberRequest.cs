using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PhoneNumberRequest
    {
        [Required]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$",ErrorMessage = "Phone number is invalid")]
        public string Number { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}";
        }
    }
}
