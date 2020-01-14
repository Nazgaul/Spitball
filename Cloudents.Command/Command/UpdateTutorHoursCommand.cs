using Cloudents.Core.DTOs;
using System.Collections.Generic;
using Cloudents.Core;

namespace Cloudents.Command.Command
{
    public class UpdateTutorHoursCommand : ICommand
    {
        public UpdateTutorHoursCommand(long userId, IEnumerable<TutorDailyHours> tutorDailyHours)
        {
            UserId = userId;
            TutorDailyHours = tutorDailyHours;
        }
        public long UserId { get; }
        public IEnumerable<TutorDailyHours> TutorDailyHours { get; }
    }
}
