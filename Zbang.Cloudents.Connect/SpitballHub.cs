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
                if (!chatId.HasValue)
                {

                    chatId = Guid.NewGuid();
                    var roomCommand = new ChatCreateRoomCommand(new[] { Context.User.GetUserId(), userId }, chatId.Value);
                    m_WriteService.AddChatRoom(roomCommand);
                    //var t2 = Groups.Add(Context.ConnectionId, chatId.Value.ToString());
                    //var t3 = Groups.Add(userId.ToString(), chatId.Value.ToString());
                    Clients.Caller.chatRoomId(chatId.Value);

                    Clients.User(userId.ToString()).chatRoom(chatId.Value, Context.User.GetUserId().ToString());
                    //await Task.WhenAll(t1 /*, t2, t3*/);
                    //Clients.Group(chatId.Value.ToString()).assignGroup(chatId.Value.ToString());
                }
                var messageCommand = new ChatAddMessageCommand(chatId.Value, Context.User.GetUserId(), message);
                m_WriteService.AddChatMessage(messageCommand);
                //Clients.OthersInGroup(chatId.Value.ToString());
                Clients.Users(usersToSend).chat(messageCommand.Message, chatId);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
        }

        public override Task OnConnected()
        {

            var user = Context.User.GetUserId();
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