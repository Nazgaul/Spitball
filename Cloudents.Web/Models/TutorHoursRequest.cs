using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Cloudents.Web.Models
{
    public class TutorHoursRequest
    {
        [Required]
        public IEnumerable<TutorDailyHoursRequest> TutorDailyHours { get; set; }
    }

    public class TutorDailyHoursRequest : IValidatableObject
    {
        [Required]
        public DayOfWeek Day { get; set; }
        //[Required]
        //public IList<TimeSpan> TimeFrames { get; set; }

        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }

        public int Offset { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (From > To)
            {
                yield return new ValidationResult("TimeFrames need to have even length");
            }
            //if (TimeFrames.Count % 2 != 0)
            //{
            //    yield return new ValidationResult("TimeFrames need to have even length");
            //}
        }
    }

}
