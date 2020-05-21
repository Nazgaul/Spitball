using System;
using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class ChatDto
    {
        public ChatDto()
        {
            Users = new List<ChatUserDto>();
        }
        public IList<ChatUserDto> Users { get; set; }

        [EntityBind(nameof(ChatRoom.Identifier))]
        public string ConversationId { get; set; }

        [EntityBind(nameof(ChatRoom.UpdateTime))]
        public DateTime DateTime { get; set; }

        [EntityBind(nameof(ChatMessage))]
        public string LastMessage { get; set; }
    }
}