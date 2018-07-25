
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cloudents.Web.Models
{
    public class CreateRedeemRequest
    {
        [RedeemValidator]
        public decimal Amount { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
    public sealed class RedeemValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val = (decimal)value;

            var validValues = new[] { 1000M, 2000M, 3000M, 4000M };

            if (validValues.Contains(val))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}