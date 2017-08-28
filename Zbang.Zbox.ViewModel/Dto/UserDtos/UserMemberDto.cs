using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserMemberDto
    {
        public UserRelationshipType UserStatus { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }

        public int Badges { get; set; }
        public int Score { get; set; }
    }
}
