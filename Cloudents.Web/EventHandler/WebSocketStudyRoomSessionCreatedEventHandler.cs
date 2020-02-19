using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketStudyRoomSessionCreatedEventHandler
        : IEventHandler<StudyRoomSessionCreatedEvent>,
            IEventHandler<StudyRoomSessionRejoinEvent>
    //IEventHandler<StudyRoomOnlineChangeEvent>
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
            var studyRoomSession = eventMessage.StudyRoomSession;
            await DoProcessAsync(studyRoomSession, token);
        }

        private async Task DoProcessAsync(StudyRoomSession studyRoomSession, CancellationToken token)
        {
            var users = studyRoomSession.StudyRoom.Users;
            var session = studyRoomSession.SessionId;
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

        public async Task HandleAsync(StudyRoomSessionRejoinEvent eventMessage, CancellationToken token)
        {
            var studyRoomSession = eventMessage.StudyRoomSession;
            await DoProcessAsync(studyRoomSession, token);
        }
    }
}
