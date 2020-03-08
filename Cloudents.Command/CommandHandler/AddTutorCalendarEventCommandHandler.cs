using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddTutorCalendarEventCommandHandler : ICommandHandler<AddTutorCalendarEventCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly ICalendarService _calendarService;
        private readonly IRepository<GoogleTokens> _googleTokenRepository;
        private readonly IJsonSerializer _jsonSerializer;

        public AddTutorCalendarEventCommandHandler(IRegularUserRepository userRepository, ITutorRepository tutorRepository, 
            ICalendarService calendarService, IRepository<GoogleTokens> googleTokenRepository, IJsonSerializer jsonSerializer)
        {
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
            _calendarService = calendarService;
            _googleTokenRepository = googleTokenRepository;
            _jsonSerializer = jsonSerializer;
        }

        public async Task ExecuteAsync(AddTutorCalendarEventCommand message, CancellationToken token)
        {
            //TODO : need to check universal time in all the process in here
            //TODO : need to check only one hour is booked
            //TODO : need to check if user have payment detail
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            if (!tutor.TutorHours.Any(a => a.AvailabilitySlot.Day == message.From.DayOfWeek
                                           && a.AvailabilitySlot.From <= message.From.TimeOfDay
                                           && message.To.TimeOfDay <= a.AvailabilitySlot.To))
            {
                throw new ArgumentException("Slot is booked");
            }
            // Tutor hours
            var appointments = await _calendarService.ReadCalendarEventsAsync(tutor.Id, tutor.Calendars.Select(s => s.Calendar.GoogleId), message.From.AddHours(-1), message.To.AddHours(1), token);
            if (appointments.Any(a =>
            {
                if (IsBetween(message.From, a.From, a.To))
                {
                    return true;
                }
                if (IsBetween(message.To, a.From, a.To))
                {
                    return true;
                }

                return false;
            }))
            {
                throw new ArgumentException("Google Slot is booked");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            var tutorToken = _jsonSerializer.Deserialize<GoogleTokensValue>((await _googleTokenRepository.GetAsync(message.TutorId.ToString(), token)).Value);
            //var userToken = await _googleTokenRepository.LoadAsync(message.UserId, token);

            await _calendarService.BookCalendarEventAsync(tutor.User, user, tutorToken.Id_token,
                message.From, message.To, token);

        }

        private static bool IsBetween(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck > startDate && dateToCheck < endDate;
        }

        private class GoogleTokensValue
        {

            public string Access_token { get; set; }
            public string Token_type { get; set; }
            public string Expires_in { get; set; }
            public string Refresh_token { get; set; }
            public string Scope { get; set; }
            public string Id_token { get; set; }
            public string Issued { get; set; }
            public string IssuedUtc { get; set; }
        }
    }
}