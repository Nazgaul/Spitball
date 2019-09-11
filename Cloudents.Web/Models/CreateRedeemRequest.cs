
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Models
{




    public sealed class BuyPointsRequest : IValidatableObject
    {
        [Required]
        public int Points { get; set; }


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
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            try
            {
                PointBundle.Parse(Points);
            }
            catch (ArgumentException)
            {
                return new[] { new ValidationResult("Invalid amount") };
            }
            return new[]
            {
                ValidationResult.Success
            };
            //foreach (var field in typeof(PointBundle).GetFields(BindingFlags.Public | BindingFlags.Static))
            //{
            //    if (field.IsLiteral)
            //    {
            //        continue;
            //    }
            //    var bundle =  (PointBundle)field.GetValue(null);

            //    if (bundle.Amount != Amount)
            //    {
            //        yield return new ValidationResult("Invalid amount");
            //    }
            //}
        }
    }
}