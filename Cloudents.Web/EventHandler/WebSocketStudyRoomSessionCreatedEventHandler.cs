using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Google.Apis.Docs.v1.Data;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketStudyRoomSessionCreatedEventHandler : IEventHandler<StudyRoomSessionCreatedEvent>
    {
        private readonly IHubContext<StudyRoomHub> _hubContext;
        private readonly IVideoProvider _videoProvider;

        public WebSocketStudyRoomSessionCreatedEventHandler(IHubContext<StudyRoomHub> hubContext, IVideoProvider videoProvider)
        {
            _hubContext = hubContext;
            _videoProvider = videoProvider;
        }

        public async Task HandleAsync(StudyRoomSessionCreatedEvent eventMessage, CancellationToken token)
        {
            var users = eventMessage.StudyRoomSession.StudyRoom.Users;
            var session = eventMessage.StudyRoomSession.SessionId;
            var tasks = new List<Task>();
            foreach (var user in users)
            {
                var jwtToken = await _videoProvider.ConnectToRoomAsync(session, user.User.Id.ToString());
                var message = new SignalRTransportType(SignalRType.StudyRoom,
                    SignalREventAction.StartSession, new
                    {
                        jwtToken
                    });

                var t = _hubContext.Clients.User(user.User.Id.ToString()).SendAsync(SbHub.MethodName, message, token);
                tasks.Add(t);
            }

            await Task.WhenAll(tasks);

        }
    }
}