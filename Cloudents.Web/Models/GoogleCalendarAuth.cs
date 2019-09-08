using System;

namespace Cloudents.Web.Models
{
    public class GoogleCalendarAuth
    {
        public string Code { get; set; }
    }

    public class SetCalendarRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class CalendarEventRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public long TutorId { get; set; }
    }
}
