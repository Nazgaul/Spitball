using System.Collections.Generic;
using Cloudents.Core;

namespace Cloudents.Command.Command
{
    public class SetTutorHoursCommand : ICommand
    {
        public SetTutorHoursCommand(long userId, IEnumerable<TutorAvailabilitySlot> tutorDailyHours)
        {
            UserId = userId;
            TutorDailyHoursObj = tutorDailyHours;
        }
        public long UserId { get; }
        public IEnumerable<TutorAvailabilitySlot> TutorDailyHoursObj { get; }
    }


}
