using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class ConversationDto
    {
        [EntityBind(nameof(ChatRoom.Identifier))]
        public string Id { get; set; }
        [EntityBind(nameof(ChatRoom.UpdateTime))]

        public DateTime LastMessage { get; set; }

        [EntityBind(nameof(BaseUser.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string UserPhoneNumber { get; set; }
        [EntityBind(nameof(User.Email))]
        public string UserEmail { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string TutorName { get; set; }

        [EntityBind(nameof(User.PhoneNumber))]
        public string TutorPhoneNumber { get; set; }
        [EntityBind(nameof(User.Email))]
        public string TutorEmail { get; set; }

        public ChatRoomStatus Status { get; set; }

        public int ConversationStatus { get; set; }
        public string AssignTo { get; set; }

        public long UserId { get; set; }
        public long TutorId { get; set; }
        public string RequestFor { get; set; }

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
        [EntityBind(nameof(BaseUser.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(BaseUser.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string? Image { get; set; }
    }
}
