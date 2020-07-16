using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Interfaces;
using NCrontab;

namespace Cloudents.Infrastructure
{
    public class CronService : ICronService
    {
        public DateTime CalculateEndTime(DateTime start, string cronSchedule, int endAfter)
        {
            var schedule = CrontabSchedule.Parse(cronSchedule);
            var nextOccurrences = schedule.GetNextOccurrences(start, start.AddDays(7 * endAfter));
            return nextOccurrences.Take(endAfter).Last().AddMinutes(15);
        }

        public string BuildCronDaily(DateTime baseDate)
        {
            return $"{baseDate.Minute} {baseDate.Hour} * * *";
        }

        public string BuildCronWeekly(DateTime baseDate)
        {
            return $"{baseDate.Minute} {baseDate.Hour} * * {baseDate.DayOfWeek}";
        }

        public string BuildCronCustom(DateTime baseDate, IEnumerable<DayOfWeek> days)
        {
           var dayStr = string.Join(",", days.Union(new []{baseDate.DayOfWeek}).Distinct().Select(s=> (int)s));
           return $"{baseDate.Minute} {baseDate.Hour} * * {dayStr}";
        }

        public DateTime GetNextOccurrence(string cronSchedule)
        {
            var schedule = CrontabSchedule.Parse(cronSchedule);
            return schedule.GetNextOccurrence(DateTime.UtcNow);
        }
    }
}
