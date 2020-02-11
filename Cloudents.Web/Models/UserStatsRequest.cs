using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cloudents.Web.Models
{
    public class UserStatsRequest : IValidatableObject
    {
        public int Days { get; set; }
        private static readonly int[] validDays = { 7, 30, 90 };
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (!validDays.Contains(Days))
            {
                yield return new ValidationResult("Invalid input");
            }
        }
    }
}
