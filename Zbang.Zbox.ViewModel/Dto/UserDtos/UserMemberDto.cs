using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class UserMemberDto : UserDto
    {
        protected UserRelationshipType UserStatus { get; set; }

        //TODO: remove this to json.net
        public string sUserStatus
        {
            get { return UserStatus.ToString("g"); }
        }
    }
}
