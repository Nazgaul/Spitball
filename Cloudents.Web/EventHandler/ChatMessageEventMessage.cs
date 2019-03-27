﻿using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace Cloudents.Web.EventHandler
{
    public class ChatMessageEventMessage : IEventHandler<ChatMessageEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;
        private readonly IChatDirectoryBlobProvider _blobProvider;
        private readonly IBinarySerializer _binarySerializer;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatMessageEventMessage(IHubContext<SbHub> hubContext, IChatDirectoryBlobProvider blobProvider, IBinarySerializer binarySerializer, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _blobProvider = blobProvider;
            _binarySerializer = binarySerializer;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(ChatMessageEvent eventMessage, CancellationToken token)
        {
            var chatMessage = eventMessage.ChatMessage;
            var message = new SignalRTransportType(SignalRType.Chat,
                SignalRAction.Add, new
                {
                    conversationId = chatMessage.ChatRoom.Id,
                    message = BuildChatMessage((dynamic)chatMessage)
                });
            var users = chatMessage.ChatRoom.Users.Select(s => s.User.Id.ToString()).ToList();
            await _hubContext.Clients.Users(users).SendAsync("Message", message, cancellationToken: token);
        }

        private ChatMessageDto BuildChatMessage(ChatTextMessage chatMessage)
        {
            return new ChatTextMessageDto
            {
                UserId = chatMessage.User.Id,
                Text = chatMessage.Message,
                DateTime = DateTime.UtcNow
            };
        }


        private ChatMessageDto BuildChatMessage(ChatAttachmentMessage chatMessage)
        {
            var url = _blobProvider.GetBlobUrl($"{chatMessage.ChatRoom.Id}/{chatMessage.Id}/{chatMessage.Blob}");

            var properties = new ImageProperties(url);
            var hash = _binarySerializer.Serialize(properties);

           var srcUrl =  _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, "imageUrl", new
            {
                hash = Base64UrlTextEncoder.Encode(hash)
            });

           var hrefUrl = _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, "ChatDownload", new
           {
               chatRoomId = chatMessage.ChatRoom.Id,
               chatId = chatMessage.Id
           });
            //return helper.RouteUrl("imageUrl", new
            //{
            //    hash = Base64UrlTextEncoder.Encode(hash)
            //});

            //yield return _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext, "Index", "Home");
            return new ChatAttachmentDto
            { 
                UserId = chatMessage.User.Id,

                Src = srcUrl,
                Href = hrefUrl,
                DateTime = DateTime.UtcNow
            };
        }
    }
}