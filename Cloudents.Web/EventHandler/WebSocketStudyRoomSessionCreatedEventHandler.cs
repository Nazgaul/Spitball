using Cloudents.Core.Entities;
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
    public class WebSocketStudyRoomSessionCreatedEventHandler
        : IEventHandler<StudyRoomSessionCreatedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;
        private readonly IStudyRoomProvider _videoProvider;

        public WebSocketStudyRoomSessionCreatedEventHandler(IHubContext<SbHub> hubContext,
            IStudyRoomProvider videoProvider)
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
            
            foreach (var user in users.Where(w => w.User.Id != studyRoomSession.StudyRoom.Tutor.Id))
            {
                var jwtToken = _videoProvider.CreateRoomToken(session, user.User.Id, user.User.Name);
                var t = _hubContext.Clients.User(user.User.Id.ToString()).SendAsync("studyRoomToken", 
                    jwtToken,studyRoomSession.StudyRoom.Id,
                    token);
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);
        }
    }
}
