using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cloudents.Web.Models
{
    public class CreateStudyRoomRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public IEnumerable<long> UserId { get; set; }

        [Required]
        [Range(0,10000000)]
        public decimal Price { get; set; }

        public DateTime? Date { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date.HasValue && Date.Value < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Date should be in the future",
                    new[] { nameof(Name) });
            }
            if (UserId?.Any() == false && !Date.HasValue)
            {
                yield return new ValidationResult(
                    "Need to enter or users or date",
                    new[] { nameof(Name) });
            }
        }
    }
}