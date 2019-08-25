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
            var user = await _userRepository.LoadAsync(message.UserId,token);

            await _calendarService.BookCalendarEventAsync(new[] {user, tutor.User}, message.From, message.To, token);
            
        }
    }
}