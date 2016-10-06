using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
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
        [Authorize]
        public async Task SendAsync(long userId, string message, Guid? chatId, string blob)
        {
            try
            {
                if (userId == Context.User.GetUserId())
                {
                    return;
                }
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

        public void UpdateImage(string blobName, IList<string> users)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                return;
            }
            Clients.Users(users).updateImage(blobName);
        }


        public void UpdateThumbnail(long itemId, long boxId)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                return;
            }
            Clients.Group($"box:{boxId}").updateThumbnail(itemId);
        }


        /// <summary>
        /// use by worker role to disconnect user
        /// </summary>
        /// <param name="userId"></param>
        public void Offline(long userId)
        {
            Clients.Others.offline(userId);
        }

        public void Echo()
        {
            Clients.Caller.echo(Context.User.GetUserId());
        }

        public void ChangeUniversity()
        {
            if (Context.User.GetUniversityId() == null)
            {
                return;
            }
            Groups.Add(Context.ConnectionId, Context.User.GetUniversityId().ToString());
            Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).online(Context.User.GetUserId());
        }

        public void EnterBoxes(long[] boxIds)
        {
            foreach (var boxId in boxIds)
            {
                Groups.Add(Context.ConnectionId, $"box:{boxId}");
            }
           
        }
        /// <summary>
        /// Api to disconnect user from signalr - use by mobile api
        /// </summary>
        public void Disconnect()
        {
            Clients.Others.offline(Context.User.GetUserId());
            var user = Context.User.GetUserId();
            try
            {
                m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, false, Context.ConnectionId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("OnDisconnected", ex);
            }
        }

        public override Task OnConnected()
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                return base.OnConnected();
            }
            var user = Context.User.GetUserId();
            if (Context.User.GetUniversityId().HasValue)
            {
                Groups.Add(Context.ConnectionId, Context.User.GetUniversityId().ToString());
                Clients.Others.online(Context.User.GetUserId());
                //Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).online(Context.User.GetUserId());
            }
            try
            {
                m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, true, Context.ConnectionId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("onconnected", ex);
            }

            return base.OnConnected();


        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                return base.OnDisconnected(stopCalled);

            }
            var user = Context.User.GetUserId();
            try
            {
                m_WriteService.ChangeOnlineStatus(new ChangeUserOnlineStatusCommand(user, false, Context.ConnectionId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("OnDisconnected", ex);
            }
            if (Context.User.GetUniversityId().HasValue)
            {
                Clients.Others.offline(Context.User.GetUserId());
                //Clients.OthersInGroup(Context.User.GetUniversityId().ToString()).offline(Context.User.GetUserId());
            }
            return base.OnDisconnected(stopCalled);
        }
    }
}