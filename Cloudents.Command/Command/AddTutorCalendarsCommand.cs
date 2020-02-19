using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class AddTutorCalendarsCommand : ICommand
    {
        public AddTutorCalendarsCommand(long tutorId, IEnumerable<Calendar> calendars)
        {
            Calendars = calendars;
            TutorId = tutorId;
        }

        public IEnumerable<Calendar> Calendars { get; }

        public long TutorId { get; }

        public class Calendar
        {
            public Calendar(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public string Id { get; }
            public string Name { get; }
        }
    }
}