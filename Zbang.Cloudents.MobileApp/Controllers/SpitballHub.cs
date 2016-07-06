using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.MobileApp.Controllers
{
   // [Authorize]
    public class SpitballHub : Hub
    {
        private readonly IZboxWriteService m_WriteService;

        public SpitballHub(IZboxWriteService writeService)
        {
            m_WriteService = writeService;
        }

        [HubMethodName("Send")]
        [Authorize]
        public async Task SendAsync(long userId, string message, Guid? chatId, string blob)
        {
            try
            {
                if (userId == Context.User.GetCloudentsUserId())
                {
                    return;
                }
                var usersToSend = new[] { userId.ToString(), Context.User.GetCloudentsUserId().ToString() };
                var messageCommand = new ChatAddMessageCommand(chatId, Context.User.GetCloudentsUserId(), message,
                    new[] { Context.User.GetCloudentsUserId(), userId }, blob);
                await m_WriteService.AddChatMessageAsync(messageCommand);
                Clients.Users(usersToSend).chat(messageCommand.Message, messageCommand.ChatRoomId, userId, blob);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        public void Offline(long userId)
        {
            Clients.Others().offline(userId);
        }

        public void ChangeUniversity()
        {
            Groups.Add(Context.ConnectionId, Context.User.GetUniversityId().ToString());
            Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).online(Context.User.GetCloudentsUserId());
        }

        public void UpdateImage(string blobName)
        {
            Clients.Others.updateImage(blobName);
        }

        public override Task OnConnected()
        {
            if (!Context.User.Identity.IsAuthenticated) return base.OnConnected();
            var user = Context.User.GetCloudentsUserId();
            if (Context.User.GetUniversityId().HasValue)
            {
                Groups.Add(Context.ConnectionId, Context.User.GetUniversityId().ToString());
                Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).online(Context.User.GetCloudentsUserId());
            }
            TraceLog.WriteInfo($"{user} is online");
            m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, true, Context.ConnectionId));
            return base.OnConnected();
        }

        

        public override Task OnDisconnected(bool stopCalled)
        {
            if (!Context.User.Identity.IsAuthenticated) return base.OnDisconnected(stopCalled);
            var user = Context.User.GetCloudentsUserId();
            m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, false, Context.ConnectionId));
            if (Context.User.GetUniversityId().HasValue)
            {
                Clients.OthersInGroup(Context.User.GetUniversityId().ToString())
                    .offline(Context.User.GetCloudentsUserId());
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}