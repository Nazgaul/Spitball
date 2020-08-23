using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Cloudents.Core.Entities;

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

        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            var modTicks = dt.Ticks % d.Ticks;
            var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

        public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;

            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }
    }

    public static class CultureInfoExtensions
    {
        public static CultureInfo? ChangeCultureBaseOnCountry(this CultureInfo cultureInfo, string country)
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

        public static CultureInfo? ChangeCultureBaseOnCountry(this CultureInfo cultureInfo, Country country)
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

    public static class LinqExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}