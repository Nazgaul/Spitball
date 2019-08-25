using System;
using System.Runtime.Serialization;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class UserDto
    {
        public UserDto(long id, string name, int score, string image)
        {
            Id = id;
            Name = name;
            Score = score;
            Image = image;
        }

        // ReSharper disable once MemberCanBeProtected.Global need that for mark answer as correct.
        public UserDto()
        {

        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
    }

    public class UserProfileDto
    {
        [EntityBind(nameof(BaseUser.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(BaseUser.Image))]
        public string Image { get; set; }
        [EntityBind(nameof(BaseUser.Score))]
        public int Score { get; set; }
        [EntityBind(nameof(University.Name))]
        public string UniversityName { get; set; }
        [EntityBind(nameof(User.Description))]
        public string Description { get; set; }
        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }
        public bool CalendarShared { get; set; }
        //[EntityBind(nameof(Tutor.Id))]
        public UserTutorProfileDto Tutor { get; set; }
    }

    public class UserTutorProfileDto
    {
        public decimal Price { get; set; }

        public float Rate { get; set; }
        public int ReviewCount { get; set; }
        [EntityBind(nameof(User.FirstName))]
        public string FirstName { get; set; }
        [EntityBind(nameof(User.LastName))]
        public string LastName { get; set; }
    }

    public class UserAccountDto
    {
        public decimal Balance { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public bool UniversityExists { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public ItemState? IsTutor { get; set; }

        public bool NeedPayment { get; set; }
    }




    public class ChatUserDto
    {
        [EntityBind(nameof(BaseUser.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(BaseUser.Image))]
        public string Image { get; set; }

        [EntityBind(nameof(ChatUser.Unread))]
        public int Unread { get; set; }

        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }

        [EntityBind(nameof(ChatRoom.Identifier))]
        public string ConversationId { get; set; }

        [EntityBind(nameof(ChatRoom.UpdateTime))]
        public DateTime DateTime { get; set; }

        [EntityBind(nameof(StudyRoom.Id))]

        public Guid? StudyRoomId { get; set; }

        public string LastMessage { get; set; }
    }
}