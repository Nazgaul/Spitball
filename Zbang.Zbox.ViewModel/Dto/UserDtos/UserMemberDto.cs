using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserMemberDto : UserDto
    {
        public UserRelationshipType UserStatus { get; set; }
        public string Email { get; set; }


    }
}
