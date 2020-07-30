using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public class SendCalendarEventEventHandler : IEventHandler<AddUserBroadcastStudyRoomEvent>
    {
        private readonly ICalendarService _calendarService;
        private readonly IUrlBuilder _urlBuilder;

        public SendCalendarEventEventHandler(ICalendarService calendarService, IUrlBuilder urlBuilder)
        {
            _calendarService = calendarService;
            _urlBuilder = urlBuilder;
        }

        public Task HandleAsync(AddUserBroadcastStudyRoomEvent eventMessage, CancellationToken token)
        {
            var studyRoom = eventMessage.BroadCastStudyRoom;
            var user = eventMessage.User;
            var eventName = //x.GetString("EnrollCalendarMessage", CultureInfo.CurrentUICulture)
                             $"Spitball Live session - {studyRoom.Course.Name}";
            eventName = string.Format(eventName, studyRoom.Course.Name);

            var url = _urlBuilder.BuildStudyRoomEndPoint(studyRoom.Id);
            return _calendarService.SendCalendarInviteAsync(
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