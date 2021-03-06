﻿using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.EventHandler
{
    public class WebSocketChatMessageEventHandler : IEventHandler<ChatMessageEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;
      //  private readonly IHubContext<StudyRoomHub> _studyRoomContext;

        private readonly IChatDirectoryBlobProvider _blobProvider;
        private readonly IBinarySerializer _binarySerializer;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
       // private readonly IUrlBuilder _urlBuilder;

        public WebSocketChatMessageEventHandler(IHubContext<SbHub> hubContext, IChatDirectoryBlobProvider blobProvider, 
            IBinarySerializer binarySerializer, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor
            //IHubContext<StudyRoomHub> studyRoomContext
            )
        {
            _hubContext = hubContext;
            _blobProvider = blobProvider;
            _binarySerializer = binarySerializer;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
           // _studyRoomContext = studyRoomContext;
            // _urlBuilder = urlBuilder;
        }

        public async Task HandleAsync(ChatMessageEvent eventMessage, CancellationToken token)
        {
            var chatMessage = eventMessage.ChatMessage;
            var message = new SignalRTransportType(SignalRType.Chat,
                SignalRAction.Add, new
                {
                    conversationId = chatMessage.ChatRoom.Identifier,
                    message = BuildChatMessage((dynamic)chatMessage)
                });
            

            List<string> users = BuildUserList((dynamic)chatMessage);
            if (users.Count > 0)
            {
                await _hubContext.Clients.Users(users).SendAsync(SbHub.MethodName, message, token);
            }
        }

        private List<string> BuildUserList(ChatTextMessage chatMessage)
        {
            return chatMessage.ChatRoom.Users.Where(w => w.User.Id != chatMessage.User.Id)
                .Select(s => s.User.Id.ToString()).ToList();

        }
        private List<string> BuildUserList(ChatAttachmentMessage chatMessage)
        {
            return chatMessage.ChatRoom.Users.Select(s => s.User.Id.ToString()).ToList();

        }
        private ChatMessageDto BuildChatMessage(ChatTextMessage chatMessage)
        {
            return new ChatTextMessageDto
            {
                UserId = chatMessage.User.Id,
                Text = chatMessage.Message,
                DateTime = DateTime.UtcNow,
                Name = chatMessage.User.Name,
               // Image = _urlBuilder.BuildUserImageEndpoint(chatMessage.User.Id, chatMessage.User.ImageName)
            };
        }


        private ChatMessageDto BuildChatMessage(ChatAttachmentMessage chatMessage)
        {
            var url = _blobProvider.GetBlobUrl($"{chatMessage.ChatRoom.Id}/{chatMessage.Id}/{chatMessage.Blob}");

            var properties = new ImageProperties(url);
            var hash = _binarySerializer.Serialize(properties);

            var srcUrl = _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, "imageUrl", new
            {
                hash = Base64UrlTextEncoder.Encode(hash)
            });

            var hrefUrl = _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, "ChatDownload", new
            {
                chatRoomId = chatMessage.ChatRoom.Id,
                chatId = chatMessage.Id
            });
            return new ChatAttachmentDto
            {
                UserId = chatMessage.User.Id,
                Name = chatMessage.User.Name,
                Src = srcUrl,
                Href = hrefUrl,
                DateTime = DateTime.UtcNow
            };
        }
    }
}