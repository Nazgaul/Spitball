using System;
using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSchedule : ValueObject
    {
        public StudyRoomSchedule(string cronString, DateTime end,DateTime start)
        {
            End = end;
            CronString = cronString ?? throw new ArgumentNullException(nameof(cronString));
            Start = start;
        }

        protected StudyRoomSchedule()
        {

        }

        public DateTime End { get;  }

        public string CronString { get; }


        public DateTime Start { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CronString;
            yield return End;
        }
    }
}