using System.Collections.Generic;
using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketStudyRoomOnlineEventHandler : IEventHandler<StudyRoomOnlineChangeEvent>
    {
        private readonly IHubContext<StudyRoomHub> _hubContext;
        private readonly IVideoProvider _videoProvider;
        private readonly TelemetryClient _telemetryClient;

        public WebSocketStudyRoomOnlineEventHandler(IHubContext<StudyRoomHub> hubContext, IVideoProvider videoProvider, TelemetryClient telemetryClient)
        {
            _hubContext = hubContext;
            _videoProvider = videoProvider;
            _telemetryClient = telemetryClient;
        }

        public async Task HandleAsync(StudyRoomOnlineChangeEvent eventMessage, CancellationToken token)
        {
            var studyUser = eventMessage.StudyUser;
            var studyRoom = studyUser.Room;

            var onlineCount = studyRoom.Users.Count(f => f.Online);
            var totalOnline = 2; // //studyRoom.Users.Count;
            _telemetryClient.TrackEvent($"Users in room {studyRoom.Id}",metrics: new Dictionary<string, double>()
            {
                ["onlineCount"] = onlineCount,
                ["totalOnline"] = totalOnline
            });
            if (onlineCount == totalOnline)
            {
                var session = studyRoom.GetCurrentSession();//.AsQueryable().Where(w => w.Ended == null).OrderByDescending(o => o.Id).FirstOrDefault();
                if (session != null)
                {
                    var roomExists = await _videoProvider.GetRoomAvailableAsync(session.SessionId);
                    if (roomExists)
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