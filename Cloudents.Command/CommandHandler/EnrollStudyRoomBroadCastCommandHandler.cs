using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Command.Resources;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class EnrollStudyRoomBroadCastCommandHandler : ICommandHandler<EnrollStudyRoomBroadCastCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<BroadCastStudyRoom> _studyRoomRepository;
        private readonly ICalendarService _calendarService;
        private readonly IUrlBuilder _urlBuilder;

        public EnrollStudyRoomBroadCastCommandHandler(IRegularUserRepository userRepository, 
            IRepository<BroadCastStudyRoom> studyRoomRepository, ICalendarService calendarService, 
            IUrlBuilder urlBuilder)
        {
            _userRepository = userRepository;
            _studyRoomRepository = studyRoomRepository;
            _calendarService = calendarService;
            _urlBuilder = urlBuilder;
        }

        public async Task ExecuteAsync(EnrollStudyRoomBroadCastCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

            studyRoom.EnrollToStudyRoom(user);
            var x = new System.Resources.ResourceManager(typeof(CalendarResources));
            var eventName = x.GetString("EnrollCalendarMessage", CultureInfo.CurrentUICulture)
                            ?? $"Spitball Live session - {studyRoom.Name}";
            eventName = string.Format(eventName, studyRoom.Name);

            var url = _urlBuilder.BuildStudyRoomEndPoint(studyRoom.Id);
            await _calendarService.SendCalendarInviteAsync(
                new[] { studyRoom.Tutor.User.Email, user.Email },
                studyRoom.BroadcastTime,
                studyRoom.BroadcastTime.AddHours(1),
                eventName,
                $"{studyRoom.Description} {url}",
                token
            );
        }
    }
}