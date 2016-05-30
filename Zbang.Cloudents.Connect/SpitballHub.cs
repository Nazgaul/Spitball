using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Connect
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
                var usersToSend = new[] { userId.ToString(), Context.User.GetUserId().ToString() };
                var messageCommand = new ChatAddMessageCommand(chatId, Context.User.GetUserId(), message,
                    new[] { Context.User.GetUserId(), userId }, blob);
                await m_WriteService.AddChatMessageAsync(messageCommand);
                Clients.Users(usersToSend).chat(messageCommand.Message, messageCommand.ChatRoomId, userId, blob);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        public void UpdateImage(string blobName)
        {
            Clients.Others.updateImage(blobName);
        }

        public override Task OnConnected()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var user = Context.User.GetUserId();
                Groups.Add(Context.ConnectionId, Context.User.GetUniversityId().ToString());
                Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).online(Context.User.GetUserId());
                m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, true));
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                var user = Context.User.GetUserId();
                m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, false));
                Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).offline(Context.User.GetUserId());
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}