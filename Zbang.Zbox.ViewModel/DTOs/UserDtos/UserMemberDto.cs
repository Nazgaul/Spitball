﻿namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class UserMemberDto : UserDto
    {
        public UserMemberDto()
        {
            
        }
        protected Zbang.Zbox.Infrastructure.Enums.UserRelationshipType UserStatus { get; set; }

        //TODO: remove this to json.net
        public string sUserStatus
        {
            get { return UserStatus.ToString("g"); }
        }
    }
}
