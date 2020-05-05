using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class CreateStudyRoomRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public IEnumerable<long> UserId { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal Price { get; set; }

        [Required]
        public StudyRoomType Type { get; set; }

        public DateTime? Date { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == StudyRoomType.Broadcast && !Date.HasValue)
            {
                yield return new ValidationResult(
                    "Need a date",
                    new[] { nameof(Name) });
            }
            if (Type == StudyRoomType.Broadcast && Date.Value < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Date should be in the future",
                    new[] { nameof(Name) });
            }
            if (Type == StudyRoomType.Private && UserId?.Any() == false)
            {
                yield return new ValidationResult(
                    "Need to enter or users",
                    new[] { nameof(Name) });
            }
        }

        public string? Description { get; set; }
        
    }

   

   
}