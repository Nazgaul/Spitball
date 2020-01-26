using Cloudents.Web.Resources;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Cloudents.Web.Models
{
    public class PhoneNumberRequest : IValidatableObject
    {
        private static readonly Regex PhoneNumberRegex = new Regex(@"^\+{1}?[1-9]\d{1,14}$", RegexOptions.Compiled);


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
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PhoneNumberRegex.IsMatch(ToString())) yield break;
            var errorMessage = "Invalid phone number";

            if (validationContext.GetService(typeof(IStringLocalizer<DataAnnotationSharedResource>)) is IStringLocalizer<DataAnnotationSharedResource> stringLocalizer)
            {
                errorMessage = stringLocalizer["InvalidPhoneNumber"];
            }

            yield return new ValidationResult(
                errorMessage,
                new[] { nameof(PhoneNumber) });
        }
    }
}
