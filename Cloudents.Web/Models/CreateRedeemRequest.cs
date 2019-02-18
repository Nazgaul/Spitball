
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateRedeemRequest
    {
        //[RedeemValidator(ErrorMessage = "InvalidRedeemValue")]
        //public decimal Amount { get; set; }

        //public override string ToString()
        //{
        //    return $"{nameof(Amount)}: {Amount}";
        //}
    }

    //[System.AttributeUsage(System.AttributeTargets.All)]
    //public sealed class RedeemValidatorAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var val = (decimal)value;

    //        var validValues = new[] { 1000M };

    //        if (validValues.Contains(val))
    //        {
    //            return ValidationResult.Success;
    //        }
    //        return new ValidationResult(ErrorMessage);
    //    }
    //}

    public sealed class PayPalTransactionRequest
    {
        [Required]
        public string Id { get; set; }
    }
}