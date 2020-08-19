using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CalendarEventRequest
    {
        [Required]
        public DateTime? From { get; set; }
        //[Required]
        public DateTime? To { get; set; }

        public long TutorId { get; set; }
    }
}