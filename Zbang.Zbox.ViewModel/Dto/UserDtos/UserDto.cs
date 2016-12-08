using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserDto
    {
        public string Image { get; set; }
        public string Name {get;set;}
        public long Id { get; set; }
        public int Badges { get; set; }
        public int Score { get; set; }

    }
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


    public class ChatUserDtoComparer : IEqualityComparer<ChatUserDto>
    {
        public bool Equals(ChatUserDto x, ChatUserDto y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ChatUserDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
