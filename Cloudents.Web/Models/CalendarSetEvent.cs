using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CalendarSetEvent
    {
        [Required]
        public DateTime? From { get; set; }

        public long TutorId { get; set; }
    }
}