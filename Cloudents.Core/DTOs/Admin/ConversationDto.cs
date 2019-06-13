﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class ConversationDto
    {
        [EntityBind(nameof(ChatRoom.Identifier))]
        public string Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(RegularUser.PhoneNumber))]
        public string UserPhoneNumber { get; set; }
        [EntityBind(nameof(RegularUser.Email))]
        public string UserEmail { get; set; }
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; }
        
        [EntityBind(nameof(RegularUser.PhoneNumber))]
        public string TutorPhoneNumber { get; set; }
        [EntityBind(nameof(RegularUser.Email))]
        public string TutorEmail { get; set; }
        public DateTime LastMessage { get; set; }
        public string Status { get; set; }
    }

    public class ConversationDetailsDto
    {
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(RegularUser.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [EntityBind(nameof(RegularUser.Image))]
        public string Image { get; set; }
    }
}
