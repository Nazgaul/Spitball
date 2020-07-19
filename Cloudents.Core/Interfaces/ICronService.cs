using System;
using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface ICronService
    {
        public DateTime CalculateEndTime(DateTime start, string cronSchedule, int endAfter);
        public string BuildCronDaily(DateTime baseDate);
        public string BuildCronWeekly(DateTime baseDate);
        public string BuildCronCustom(DateTime baseDate, IEnumerable<DayOfWeek> days );

        public DateTime GetNextOccurrence(string cronSchedule);
        public IEnumerable<DateTime> GetNextOccurrences(string cronSchedule, DateTime end);
    }
}