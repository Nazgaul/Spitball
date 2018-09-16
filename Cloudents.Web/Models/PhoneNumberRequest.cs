using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class PhoneNumberRequest : IValidatableObject
    {
        private static readonly Regex PhoneNumberRegex = new Regex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);
        [Required(ErrorMessage = "Required")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Required")]
        public int CountryCode { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder($"+{CountryCode}");
            if (PhoneNumber.StartsWith("0"))
            {
                PhoneNumber = PhoneNumber.Remove(0, 1);
            }

            sb.Append(PhoneNumber);
            return sb.ToString();
            //    return $"{nameof(PhoneNumber)}: {PhoneNumber}, {nameof(CountryCode)}: {CountryCode}";
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PhoneNumberRegex.IsMatch(ToString()))
            {
                yield return ValidationResult.Success;
            }
            yield return new ValidationResult(
                "Phone number is invalid",
                new[] { "" });
        }

        //public override string ToString()
        //{
            
        //    //return $"{nameof(Number)}: {Number}";
        //}
    }
}
