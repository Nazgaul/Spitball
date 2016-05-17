using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
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

        public async Task Send(long userId, string message, Guid? chatId)
        {
            try
            {
                if (!chatId.HasValue)
                {

                    chatId = Guid.NewGuid();
                    var roomCommand = new ChatCreateRoomCommand(new[] {Context.User.GetUserId(), userId}, chatId.Value);
                    var t1 = m_WriteService.AddChatRoomAsync(roomCommand);
                    //var t2 = Groups.Add(Context.ConnectionId, chatId.Value.ToString());
                    //var t3 = Groups.Add(userId.ToString(), chatId.Value.ToString());

                    await Task.WhenAll(t1 /*, t2, t3*/);
                    //Clients.Group(chatId.Value.ToString()).assignGroup(chatId.Value.ToString());
                }
                var messageCommand = new ChatAddMessageCommand(chatId.Value, Context.User.GetUserId(), message);
                await m_WriteService.AddChatMessageAsync(messageCommand);
                //Clients.OthersInGroup(chatId.Value.ToString());
                Clients.User(userId.ToString()).send(message);
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