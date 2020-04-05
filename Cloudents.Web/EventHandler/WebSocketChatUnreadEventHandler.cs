using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketChatUnreadEventHandler : IEventHandler<ChatReadEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public WebSocketChatUnreadEventHandler(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(ChatReadEvent eventMessage, CancellationToken token)
        {
            var chatUser = eventMessage.ChatUser;
            if (chatUser.ChatRoom.Users.All(s => s.Unread == 0))
            {
                var users = chatUser.ChatRoom.Users.Where(w => w.User.Id != chatUser.User.Id)
                    .Select(s => s.User.Id.ToString()).ToList();
                var message = new SignalRTransportType(SignalRType.Chat,
                    SignalRAction.Update, new
                    {
                        conversationId = chatUser.ChatRoom.Identifier
                    });

                await _hubContext.Clients.Users(users).SendAsync(SbHub.MethodName, message, token);
            }
        }
    }
}