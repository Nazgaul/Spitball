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
    public class WebSocketStudyRoomOnlineEventHandler : IEventHandler<StudyRoomOnlineChangeEvent>
    {
        private readonly IHubContext<StudyRoomHub> _hubContext;
        private readonly IVideoProvider _videoProvider;

        public WebSocketStudyRoomOnlineEventHandler(IHubContext<StudyRoomHub> hubContext, IVideoProvider videoProvider)
        {
            _hubContext = hubContext;
            _videoProvider = videoProvider;
        }

        public async Task HandleAsync(StudyRoomOnlineChangeEvent eventMessage, CancellationToken token)
        {
            var studyUser = eventMessage.StudyUser;
            var studyRoom = studyUser.Room;

            var onlineCount = studyRoom.Users.Count(f => f.Online);
            var totalOnline = studyRoom.Users.Count;
            if (onlineCount == totalOnline)
            {
                var session = studyRoom.Sessions.AsQueryable().Where(w => w.Ended == null).OrderByDescending(o => o.Id).FirstOrDefault();
                if (session != null)
                {
                    foreach (var user in studyRoom.Users)
                    {
                        var jwtToken =
                            await _videoProvider.ConnectToRoomAsync(session.SessionId, user.User.Id.ToString());


                        var message2 = new SignalRTransportType(SignalRType.StudyRoom,
                            SignalRAction.Update, new
                            {
                                jwtToken
                            });

                        await _hubContext.Clients.User(user.User.Id.ToString())
                                     .SendAsync(SbHub.MethodName, message2, token);
                    }

                    return;
                }
            }
            var message = new SignalRTransportType(SignalRType.StudyRoom,
                SignalRAction.Update, new
                {
                    onlineCount,
                    totalOnline
                });


            await _hubContext.Clients.Group(studyRoom.Id.ToString()).SendAsync(SbHub.MethodName, message, token);
        }
    }
}