using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetSessionDurationRequest : IValidatableObject
    {
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public long RealDuration { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (RealDuration < 10)
            {
                yield return new ValidationResult("Invalid input");
            }
        }
    }
}
