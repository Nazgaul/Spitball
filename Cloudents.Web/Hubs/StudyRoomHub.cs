using System;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Web.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class StudyRoomHub : Hub
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICommandBus _commandBus;

        public StudyRoomHub(IHttpContextAccessor httpContext, ICommandBus commandBus)
        {
            _httpContext = httpContext;
            _commandBus = commandBus;
        }

        public override async Task OnConnectedAsync()
        {
            var request = _httpContext.HttpContext.Request;
            var cookieVal = request.Cookies[StudyRoomController.CookieName];
            if (cookieVal == null)
            {
                throw new ArgumentException();
            }
            
            var roomId = Guid.Parse(cookieVal);
            var userId = long.Parse(Context.UserIdentifier);
            var command = new ChangeStudyRoomOnlineStatusCommand(userId, true, roomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, cookieVal);
            await _commandBus.DispatchAsync(command, default);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //We cant delete the cookie
            var request = _httpContext.HttpContext.Request;
            var cookieVal = request.Cookies[StudyRoomController.CookieName];

            var roomId = Guid.Parse(cookieVal);
            var userId = long.Parse(Context.UserIdentifier);
            var command = new ChangeStudyRoomOnlineStatusCommand(userId, false, roomId);
            await _commandBus.DispatchAsync(command, default);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cookieVal);
            await base.OnDisconnectedAsync(exception);
        }
    }
}