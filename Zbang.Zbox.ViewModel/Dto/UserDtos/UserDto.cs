using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    [Serializable]
    public class UserDto
    {
        public string Image { get; set; }
        public string Name {get;set;}
        public long Id { get; set; }

        public string Url { get; set; }

    }
    [Serializable]
    public class UserMinProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }
    }

    public class UserWithImageDto
    {
        public long Id { get; set; }
        public string Image { get; set; }
    }

    public class UserWithImageNameDto
    {
        public long Id { get; set; }
        public string Image { get; set; }

        public string Name { get; set; }
    }


    public class UserWithStatusDto 
    {
        public long Id { get; set; }
        public string Image { get; set; }

        public string Name { get; set; }
        
        public string Url { get; set; }

        public bool Online { get; set; }

        public Guid? Conversation { get; set; }

        public int? Unread { get; set; }
    }

    public class UserWithStatusDtoComparer : IEqualityComparer<UserWithStatusDto>
    {
        public bool Equals(UserWithStatusDto x, UserWithStatusDto y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(UserWithStatusDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
