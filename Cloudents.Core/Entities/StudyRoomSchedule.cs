using System;
using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSchedule : ValueObject
    {
        public StudyRoomSchedule(string cronString, DateTime end)
        {
            End = end;
            CronString = cronString ?? throw new ArgumentNullException(nameof(cronString));
        }

        protected StudyRoomSchedule()
        {

        }

        public DateTime End { get;  }

        public string CronString { get; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CronString;
            yield return End;
        }
    }
}