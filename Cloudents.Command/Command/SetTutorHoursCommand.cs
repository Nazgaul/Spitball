using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class SetTutorHoursCommand : ICommand
    {
        public SetTutorHoursCommand(long userId, IEnumerable<TutorDailyHours> tutorDailyHours)
        {
            UserId = userId;
            TutorDailyHoursObj = tutorDailyHours;
        }
        public long UserId { get; }
        public IEnumerable<TutorDailyHours> TutorDailyHoursObj { get; }


        public class TutorDailyHours 
        {
            public TutorDailyHours(DayOfWeek day, TimeSpan @from, TimeSpan to)
            {
                Day = day;
                From = @from;
                To = to;
            }

            public DayOfWeek Day { get; }

            public TimeSpan From { get;}
            public TimeSpan To { get; }
           // public IList<TimeSpan> TimeFrames { get; protected set; }
            //protected override IEnumerable<object> GetEqualityComponents()
            //{
            //    yield return Day;
            //    yield return TimeFrames;
            //}
        }
    }


}
