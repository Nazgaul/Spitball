using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.EventHandler
{
    /// <summary>
    /// This is an event handler if the admin sends the user a message - we then transfer it to the service bus to handle this.
    /// </summary>
    public class WebSocketChatMessageEventHandler : IEventHandler<ChatMessageEvent>
    {
        private readonly IServiceBusProvider _queueProvider;
        private readonly IUrlBuilder _urlBuilder;

        public WebSocketChatMessageEventHandler(IServiceBusProvider queueProvider, IUrlBuilder urlBuilder)
        {
            _queueProvider = queueProvider;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(ChatMessageEvent eventMessage, CancellationToken token)
        {
            var chatMessage = eventMessage.ChatMessage;
            if (chatMessage is ChatTextMessage chatTextMessage)
            {
                var message = new SignalRTransportType(SignalRType.Chat,
                    SignalRAction.Add, new
                    {
                        conversationId = chatMessage.ChatRoom.Identifier,
                        message = BuildChatMessage(chatTextMessage)
                    });

                var users = BuildUserList(chatTextMessage);
                foreach (var user in users)
                {
                    await _queueProvider.InsertMessageAsync(message, user, token);
                }

            }
        }

        private List<long> BuildUserList(ChatTextMessage chatMessage)
        {
            return chatMessage.ChatRoom.Users.Where(w => w.User.Id != chatMessage.User.Id).Select(s => s.User.Id).ToList();

        }

        private ChatMessageDto BuildChatMessage(ChatTextMessage chatMessage)
        {
            return new ChatTextMessageDto
            {
                UserId = chatMessage.User.Id,
                Text = chatMessage.Message,
                DateTime = DateTime.UtcNow,
                Name = chatMessage.User.Name,
                Image = _urlBuilder.BuildUserImageEndpoint(chatMessage.User.Id,chatMessage.User.ImageName)
            };
        }



    }
}