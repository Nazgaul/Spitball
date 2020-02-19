using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Cloudents.Core.Extension
{
    public static class DateTimeHelpers
    {
        public static IEnumerable<DateTime> EachHour(DateTime from, DateTime to)
        {
            for (var day = from; day < to; day = day.AddHours(1))
            {
                yield return day;
            }
        }

        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }
    }

    public static class CultureInfoExtensions
    {
        public static CultureInfo ChangeCultureBaseOnCountry(this CultureInfo cultureInfo, string country)
        {
            if (cultureInfo is null)
            {
                return null;
            }
            if (country == null)
            {
                Debug.Assert(country == null, "this should not happen");
                return cultureInfo;
            }
            return new CultureInfo($"{cultureInfo.ToString().Split('-')[0]}-{country}");
        }
    }
}