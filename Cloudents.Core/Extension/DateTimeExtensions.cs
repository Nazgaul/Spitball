using System;
using System.Collections.Generic;

namespace Cloudents.Core.Extension
{
    public static class DateTimeHelpers
    {
        //public static DateTime NextRandomDate(int rangeInDays, Random rand = null)
        //{
        //    if (rand == null)
        //    {
        //        rand = new Random();
        //    }

        //    rangeInDays = Math.Abs(rangeInDays);
        //    var start = DateTime.UtcNow.AddDays(-rangeInDays);

        //    return start.AddDays(rand.Next(rangeInDays))
        //                .AddHours(rand.Next(0, 24))
        //                .AddMinutes(rand.Next(0, 60))
        //                .AddSeconds(rand.Next(0, 60));
        //}

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