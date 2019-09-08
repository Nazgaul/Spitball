using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class AddTutorCalendarEventCommandHandler : ICommandHandler<AddTutorCalendarEventCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly ICalendarService _calendarService;

        public AddTutorCalendarEventCommandHandler(IRegularUserRepository userRepository, ITutorRepository tutorRepository, ICalendarService calendarService)
        {
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
            _calendarService = calendarService;
        }

        public async Task ExecuteAsync(AddTutorCalendarEventCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId,token);

            
            var appointments = await _calendarService.ReadCalendarEventsAsync(tutor.Id, tutor.Calendars.Select(s => s.GoogleId), message.From, message.To, token);
            if (appointments.BusySlot.Any())
            {
                throw new ArgumentException("Slot is booked");
            }

            var user = await _userRepository.LoadAsync(message.UserId,token);

            await _calendarService.BookCalendarEventAsync(tutor.User,user, 
                message.From, message.To, token);
            
        }
    }
}