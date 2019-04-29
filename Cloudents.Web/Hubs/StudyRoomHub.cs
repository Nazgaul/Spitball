﻿using System;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class StudyRoomHub : Hub
    {
        private readonly ICommandBus _commandBus;
        private const string CookieName = "studyRoomId";

        public StudyRoomHub( ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var request = httpContext.Request;
            var cookieVal = request.Query["studyRoomId"].ToString();
            if (cookieVal == null)
            {
              return;
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
            var request = Context.GetHttpContext().Request;
            var cookieVal = request.Query["studyRoomId"].ToString();

            var roomId = Guid.Parse(cookieVal);
            var userId = long.Parse(Context.UserIdentifier);
            var command = new ChangeStudyRoomOnlineStatusCommand(userId, false, roomId);
            await _commandBus.DispatchAsync(command, default);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cookieVal);
            await base.OnDisconnectedAsync(exception);
        }
    }
}