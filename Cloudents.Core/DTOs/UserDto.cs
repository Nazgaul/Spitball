using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

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
        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.Image))]
        public string Image { get; set; }
        [EntityBind(nameof(User.Score))]
        public int Score { get; set; }
        [EntityBind(nameof(University.Name))]
        public string UniversityName { get; set; }
        [EntityBind(nameof(RegularUser.Description))]
        public string Description { get; set; }
        [EntityBind(nameof(RegularUser.Online))]
        public bool Online { get; set; }

        //[EntityBind(nameof(Tutor.Id))]
        public UserTutorProfileDto Tutor { get; set; }
    }

    public class UserTutorProfileDto
    {
        public decimal Price { get; set; }

        public float Rate { get; set; }
        public int ReviewCount { get; set; }
        [EntityBind(nameof(RegularUser.FirstName))]
        public string FirstName { get; set; }
        [EntityBind(nameof(RegularUser.LastName))]
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
        public bool IsTutor { get; set; }

        public bool NeedPayment => false;

    }


    public class ChatUserDto
    {
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.Image))]
        public string Image { get; set; }

        [EntityBind(nameof(ChatUser.Unread))]
        public int Unread { get; set; }

        [EntityBind(nameof(RegularUser.Online))]
        public bool Online { get; set; }

        [EntityBind(nameof(ChatRoom.Id))]
        public Guid ConversationId { get; set; }

        [EntityBind(nameof(ChatRoom.UpdateTime))]
        public DateTime DateTime { get; set; }

        [EntityBind(nameof(StudyRoom.Id))]

        public Guid? StudyRoomId { get; set; }

        public string LastMessage { get; set; }
    }
}