using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
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
            var users = new List<string>()
            {
                studyRoom.Tutor.Id.ToString(),

            };
            users.AddRange(studyRoom.Users.Select(s => s.User.Id.ToString()));
            var message = new SignalRTransportType(SignalRType.StudyRoom,
                SignalRAction.Add, new
                {
                    id = studyRoom.Id
                });
            await _hubContext.Clients.Users(users).SendAsync(SbHub.MethodName, message, token);
        }
    }
}