using Cloudents.Command;
using Cloudents.Command.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class StudyRoomHub : Hub
    {
        private readonly ICommandBus _commandBus;
        private const string QueryStringName = "studyRoomId";
      
        public StudyRoomHub(ICommandBus commandBus)
        {
            _commandBus = commandBus;
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

         //   var roomId = Guid.Parse(cookieVal);
          //  var userId = long.Parse(Context.UserIdentifier);
          //  var command = new AddUserToChatCommand(roomId,userId);

            await Groups.AddToGroupAsync(Context.ConnectionId, cookieVal);
           // await _commandBus.DispatchAsync(command, default);
           
            await base.OnConnectedAsync();


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //We cant delete the cookie
            var request = Context.GetHttpContext().Request;
            var cookieVal = request.Query[QueryStringName].ToString();

         //   var roomId = Guid.Parse(cookieVal);
            //var userId = long.Parse(Context.UserIdentifier);
           // var command = new ChangeStudyRoomOnlineStatusCommand(userId, false, roomId);
           // await _commandBus.DispatchAsync(command, default);
            //await Clients.All.SendAsync("Offline", userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cookieVal);
            await base.OnDisconnectedAsync(exception);
        }
    }
}