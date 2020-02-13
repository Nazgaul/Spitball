using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Cloudents.Admin2.Models
{
    public class UpdatePhoneRequest : IValidatableObject
    {

        private static readonly Regex PhoneNumberRegex = new Regex(@"^\+{1}?[1-9]\d{1,14}$", RegexOptions.Compiled);

        public long UserId { get; set; }
        public string NewPhone { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PhoneNumberRegex.IsMatch(NewPhone)) yield break;
            var errorMessage = "Invalid phone number";

            yield return new ValidationResult(
                errorMessage,
                new[] { nameof(NewPhone) });
        }

    }
}
