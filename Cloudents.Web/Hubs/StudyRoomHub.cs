using Cloudents.Command;
using Cloudents.Command.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class StudyRoomHub : Hub
    {
        private readonly ICommandBus _commandBus;
        private const string QueryStringName = "studyRoomId";
        private readonly IHubContext<SbHub> _hubContext;
        public StudyRoomHub(ICommandBus commandBus, IHubContext<SbHub> hubContext)
        {
            _commandBus = commandBus;
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var request = httpContext.Request;
            var cookieVal = request.Query[QueryStringName].ToString();
            if (string.IsNullOrEmpty(cookieVal))
            {
                return;
            }

            var roomId = Guid.Parse(cookieVal);
            var userId = long.Parse(Context.UserIdentifier);


            var message = new SignalRTransportType(SignalRType.User,
                SignalREventAction.EnterStudyRoom, new object());

            var command = new ChangeStudyRoomOnlineStatusCommand(userId, true, roomId);

           


            await Clients.All.SendAsync("Online", userId);
            await Groups.AddToGroupAsync(Context.ConnectionId, cookieVal);
            await _commandBus.DispatchAsync(command, default);


            await _hubContext.Clients.Users(command.OtherUsers.Select(s => s.ToString()).ToList())
                .SendAsync(SbHub.MethodName, message, CancellationToken.None);
            await base.OnConnectedAsync();


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //We cant delete the cookie
            var request = Context.GetHttpContext().Request;
            var cookieVal = request.Query[QueryStringName].ToString();

            var roomId = Guid.Parse(cookieVal);
            var userId = long.Parse(Context.UserIdentifier);
            var command = new ChangeStudyRoomOnlineStatusCommand(userId, false, roomId);
            await _commandBus.DispatchAsync(command, default);
            await Clients.All.SendAsync("Offlie", userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cookieVal);
            await base.OnDisconnectedAsync(exception);
        }
    }
}