using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class GoogleCalendarAuth
    {
        public string Code { get; set; }
    }

    //public class SetCalendarRequest
    //{
    //    public string Calendar { get; set; }
    //}

    public class CalendarEventRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public long TutorId { get; set; }
    }
}
