
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{




    public sealed class BuyPointsRequest : IValidatableObject
    {
        [Required]
        public int Points { get; set; }

       
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
        }
    }
}