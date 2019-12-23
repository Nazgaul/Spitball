using System;

namespace Cloudents.Core.DTOs
{
    public class TutorDailyHoursDto
    {
        public DayOfWeek WeekDay { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}
