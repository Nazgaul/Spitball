using Cloudents.Core.DTOs;
using System.Collections.Generic;
using Cloudents.Core;

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
    }


}
