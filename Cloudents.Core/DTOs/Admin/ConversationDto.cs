using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Admin
{
    public class ConversationDto
    {
        [EntityBind(nameof(ChatRoom.Identifier))]
        public string Id { get; set; }
        [EntityBind(nameof(ChatRoom.UpdateTime))]

        public DateTime LastMessage { get; set; }

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

        public ChatRoomStatus Status { get; set; }

        private int ConversationStatus { get; set; }

        public string AutoStatus
        {
            get
            {
                if (ConversationStatus == 1)
                {
                    return "Tutor";
                }

                if (ConversationStatus == 2)
                {
                    return "Student";
                }

                return $"Conv ({ConversationStatus})";

            }
        }

        public bool StudyRoomExists { get; set; }
        public int HoursFromLastMessage { get; set; }
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
