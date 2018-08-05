using System;

namespace Cloudents.Core.Extension
{
    public static class DateTimeHelpers
    {
        public static DateTime NextRandomDate(int rangeInDays, Random rand = null)
        {
            if (rand == null)
            {
                rand = new Random();
            }

            rangeInDays = Math.Abs(rangeInDays);
            var start = DateTime.UtcNow.AddDays(-rangeInDays);

            return start.AddDays(rand.Next(rangeInDays)).AddHours(rand.Next(0, 24)).AddMinutes(rand.Next(0, 60)).AddSeconds(rand.Next(0, 60));
        }
    }
}