using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Admin2.EventHandler
{
    public class WebSocketChatMessageEventHandler : IEventHandler<ChatMessageEvent>
    {
        private readonly IServiceBusProvider _queueProvider;

        public WebSocketChatMessageEventHandler(IServiceBusProvider queueProvider)
        {
            _queueProvider = queueProvider;
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

                List<long> users = BuildUserList(chatTextMessage);
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
                Image = chatMessage.User.Image
            };
        }


        
    }
}