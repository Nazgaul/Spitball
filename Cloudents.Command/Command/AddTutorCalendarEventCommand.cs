using System;

namespace Cloudents.Command.Command
{
    public class AddTutorCalendarEventCommand : ICommand
    {
        public AddTutorCalendarEventCommand(long userId, long tutorId, DateTime @from, DateTime to)
        {
            UserId = userId;
            TutorId = tutorId;
            From = @from.ToUniversalTime();
            To = to.ToUniversalTime();
        }

        public long UserId { get;  }
        public long TutorId { get;  }
        public DateTime From { get;  }
        public DateTime To { get;  }
    }
}