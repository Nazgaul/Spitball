using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class UserDto
    {
        public UserDto(long id, string name, int score)
        {
            Id = id;
            Name = name;
            Score = score;
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
        
    }


    public class ChatUserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public int Unread { get; set; }

        public bool Online { get; set; }
    }
}