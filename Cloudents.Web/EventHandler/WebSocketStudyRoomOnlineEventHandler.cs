using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketStudyRoomOnlineEventHandler : IEventHandler<StudyRoomOnlineChangeEvent>
    {
        private readonly IHubContext<StudyRoomHub> _hubContext;

        public WebSocketStudyRoomOnlineEventHandler(IHubContext<StudyRoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(StudyRoomOnlineChangeEvent eventMessage, CancellationToken token)
        {
            var studyUser = eventMessage.StudyUser;
            var studyRoom = studyUser.Room;
            var message = new SignalRTransportType(SignalRType.StudyRoom,
                SignalRAction.Update, new
                {
                    onlineCount = studyRoom.Users.Count(f => f.Online),
                    totalOnline = studyRoom.Users.Count
                });


            await _hubContext.Clients.Group(studyRoom.Id.ToString()).SendAsync(SbHub.MethodName, message, token);
        }
    }
}