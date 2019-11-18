using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddTutorCalendarsCommandHandler : ICommandHandler<AddTutorCalendarsCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly ICalendarService _calendarService;

        public AddTutorCalendarsCommandHandler(ITutorRepository tutorRepository, ICalendarService calendarService)
        {
            _tutorRepository = tutorRepository;
            _calendarService = calendarService;
        }

        public async Task ExecuteAsync(AddTutorCalendarsCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.GetAsync(message.TutorId, token);
            var calendars = await _calendarService.GetUserCalendarsAsync(message.TutorId, token);



            foreach (var calendar in message.Calendars.Where(w => calendars.Any(a => a.Id == w.Id)))
            {
                tutor.AddCalendar(calendar.Id, calendar.Name);
            }

            await _tutorRepository.UpdateAsync(tutor, token);
        }
    }
}