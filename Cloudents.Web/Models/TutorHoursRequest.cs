using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Cloudents.Web.Models
{
    public class TutorHoursRequest
    {
        [Required]
        public IEnumerable<TutorDailyHoursRequest> TutorDailyHours { get; set; }
    }
}
