using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class ConversationDto
    {
        [EntityBind(nameof(ChatRoom.Id))]
        public Guid Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName1 { get; set; }
        public bool IsTotur1 { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName2 { get; set; }
        public bool IsTotur2 { get; set; }
        [EntityBind(nameof(ChatMessage.CreationTime))]
        public DateTime LastMessage { get; set; }
    }

    public class ConversationDetailsDto
    {
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(RegularUser.PhoneNumber))]
        public string PhoneNumber { get; set; }
    }
}
