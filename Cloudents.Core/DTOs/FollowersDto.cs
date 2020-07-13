using System;
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class FollowersDto
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public DateTime Created { get; set; }
        public string Email { get; set; }
       // public string? PhoneNumber { get; set; }

        public bool HasCreditCard { get; set; }
    }


    public class DashboardCalendarDto
    {
        public bool CalendarShared { get; set; }

        public IEnumerable<TutorAvailabilitySlot> TutorDailyHours { get; set; }
    }
}
