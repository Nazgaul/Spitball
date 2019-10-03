using System;
using System.Collections.Generic;

namespace Cloudents.Core.Extension
{
    public static class DateTimeHelpers
    {
        public static IEnumerable<DateTime> EachHour(DateTime from, DateTime to)
        {
            for (var day = from;  day < to; day = day.AddHours(1))
            {
                yield return day;
            }
        }

        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }
    }
}