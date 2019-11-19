using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.EventHandler
{

    public class WebSocketStudyRoomCreatedEventHandler : IEventHandler<StudyRoomCreatedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public WebSocketStudyRoomCreatedEventHandler(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(StudyRoomCreatedEvent eventMessage, CancellationToken token)
        {

            var studyRoom = eventMessage.StudyRoom;
            var message = new SignalRTransportType(SignalRType.StudyRoom,
                SignalRAction.Add, new
                {
                    id = studyRoom.Id,
                    userId = studyRoom.Tutor.Id,
                    conversationId = studyRoom.Identifier
                });
            await _hubContext.Clients.Users(studyRoom.Users.Select(s => s.User.Id.ToString()).ToList()).SendAsync(SbHub.MethodName, message, token);
        }
    }
}