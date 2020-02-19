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
    public class WebSocketStudyRoomOnlineUserEventHandler : IEventHandler<StudyRoomOnlineChangeEvent>
    {
        private readonly IHubContext<SbHub> _hubContextSpitball;

        public WebSocketStudyRoomOnlineUserEventHandler(IHubContext<SbHub> hubContextSpitball)
        {
            _hubContextSpitball = hubContextSpitball;
        }

        public async Task HandleAsync(StudyRoomOnlineChangeEvent eventMessage, CancellationToken token)
        {
            var studyUser = eventMessage.StudyUser;
            if (!studyUser.Online)
            {
                return;
            }
            var studyRoom = studyUser.Room;

            var otherUsers = studyRoom.Users.Select(s => s.User.Id)
                .Where(w => w != studyUser.User.Id).ToList();


            var message = new SignalRTransportType(SignalRType.User,
                SignalREventAction.EnterStudyRoom, new
                {
                    userName = studyUser.User.Name,
                    studyRoomId = studyRoom.Id
                });

            await _hubContextSpitball.Clients.Users(otherUsers.Select(s => s.ToString()).ToList())
                .SendAsync(SbHub.MethodName, message, CancellationToken.None);
        }
    }
}