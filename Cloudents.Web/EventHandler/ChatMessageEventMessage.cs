﻿using System;
using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;

namespace Cloudents.Web.EventHandler
{
    public class ChatMessageEventMessage : IEventHandler<ChatMessageEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;

        public ChatMessageEventMessage(IHubContext<SbHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(ChatMessageEvent eventMessage, CancellationToken token)
        {
            var chatMessage = eventMessage.ChatMessage;
            var message = new SignalRTransportType(SignalRType.Chat,
                SignalRAction.Add, new
                {
                    conversationId = chatMessage.ChatRoom.Id,
                    message = BuildChatMessage(eventMessage as dynamic)
                });
            var users = chatMessage.ChatRoom.Users.Select(s => s.User.Id.ToString()).ToList();
            await _hubContext.Clients.Users(users).SendAsync("Message", message, cancellationToken: token);
        }

        private ChatMessageDto BuildChatMessage(ChatTextMessage chatMessage)
        {
            return new ChatMessageDto
            {
                UserId = chatMessage.User.Id,
                Text = chatMessage.Message,
                DateTime = DateTime.UtcNow
            };
        }


        private ChatMessageDto BuildChatMessage(ChatAttachmentMessage chatMessage)
        {
            return new ChatMessageDto
            {
                UserId = chatMessage.User.Id,
               // Text = chatMessage.Message,
                DateTime = DateTime.UtcNow
            };
        }
    }
}