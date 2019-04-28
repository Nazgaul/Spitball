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
        [DtoToEntityConnection(nameof(User.Id))]
        public long Id { get; set; }
        [DtoToEntityConnection(nameof(User.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(User.Image))]
        public string Image { get; set; }
        [DtoToEntityConnection(nameof(User.Score))]
        public int Score { get; set; }
        [DtoToEntityConnection(nameof(University.Name))]
        public string UniversityName { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Description))]
        public string Description { get; set; }

        
        //[DtoToEntityConnection(nameof(Tutor.Id))]
        public UserTutorProfileDto Tutor { get; set; }
    }

    public class UserTutorProfileDto
    {
        public decimal Price { get; set; }

        public bool Online { get; set; }
        public float Rate { get; set; }
        public int ReviewCount { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.FirstName))]
        public string FirstName { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.LastName))]
        public string LastName { get; set; }
    }

    public class UserAccountDto 
    {
        public string Token { get; set; }
        public decimal Balance { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public bool UniversityExists { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public bool IsTutor { get; set; }

        public bool NeedPayment { get; set; }

    }


    public class ChatUserDto
    {
        [DtoToEntityConnection(nameof(User.Id))]
        public long UserId { get; set; }
        [DtoToEntityConnection(nameof(User.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(User.Image))]
        public string Image { get; set; }

        [DtoToEntityConnection(nameof(ChatUser.Unread))]
        public int Unread { get; set; }

        [DtoToEntityConnection(nameof(RegularUser.Online))]
        public bool Online { get; set; }

        [DtoToEntityConnection(nameof(ChatRoom.Id))]
        public Guid ConversationId { get; set; }

        [DtoToEntityConnection(nameof(ChatRoom.UpdateTime))]
        public DateTime DateTime { get; set; }

        [DtoToEntityConnection(nameof(StudyRoom.Id))]

        public Guid? StudyRoomId { get; set; }
    }
}