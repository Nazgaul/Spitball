﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Connect
{

    public class SpitballHub : Hub
    {
        private readonly IZboxWriteService m_WriteService;

        public SpitballHub(IZboxWriteService writeService)
        {
            m_WriteService = writeService;
        }

        [HubMethodName("Send")]
        public void Send(long userId, string message, Guid? chatId)
        {
            try
            {
                var usersToSend = new[] {userId.ToString(), Context.User.GetUserId().ToString()};
                //if (!chatId.HasValue)
                //{
                    //chatId = Guid.NewGuid();
                    //var roomCommand = new ChatCreateRoomCommand(new[] { Context.User.GetUserId(), userId }, chatId.Value);
                    //m_WriteService.AddChatRoom(roomCommand);
                //}
                var messageCommand = new ChatAddMessageCommand(chatId, Context.User.GetUserId(), message, new[] { Context.User.GetUserId(), userId });
                m_WriteService.AddChatMessage(messageCommand);
                //Clients.OthersInGroup(chatId.Value.ToString());
                Clients.Users(usersToSend).chat(messageCommand.Message, messageCommand.ChatRoomId, userId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        public override Task OnConnected()
        {

            var user = Context.User.GetUserId();
            Groups.Add(Context.ConnectionId, Context.User.GetUniversityId().ToString());
            m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, true));
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = Context.User.GetUserId();
            m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, false));
            return base.OnDisconnected(stopCalled);
        }
    }
}