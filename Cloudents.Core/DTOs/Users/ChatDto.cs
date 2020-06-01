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

        [EntityBind(nameof(ChatUser.Unread))]
        public int Unread { get; set; }
    }

    public class ChatConversationDetailsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public Guid? StudyRoomId { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}