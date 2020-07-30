using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class CreateLiveStudyRoomRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }


        [Required]
        [Range(0, 10000000)]
        public decimal Price { get; set; }

       
        [Required]
        public DateTime Date { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            if (Date < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Date should be in the future",
                    new[] { nameof(Name) });
            }

            if (Repeat.GetValueOrDefault() != StudyRoomRepeat.None)
            {
                if (EndAfterOccurrences.GetValueOrDefault() == 0 &&
                    EndDate.GetValueOrDefault(DateTime.MinValue) < DateTime.UtcNow)
                {
                    yield return new ValidationResult(
                        "Invalid args",
                        new[] { nameof(Name) });
                }
                if (EndAfterOccurrences.HasValue && 
                    EndDate.HasValue)
                {
                    yield return new ValidationResult(
                        "Can have both",
                        new[] { nameof(Name) });
                }
            }
        }

        [Required]
        public string Description { get; set; }

        public StudyRoomRepeat? Repeat { get; set; }

        public DateTime? EndDate { get; set; }
        public int? EndAfterOccurrences { get; set; }

        public IEnumerable<DayOfWeek> RepeatOn { get; set; }

        public string? Image { get; set; }
    }
}