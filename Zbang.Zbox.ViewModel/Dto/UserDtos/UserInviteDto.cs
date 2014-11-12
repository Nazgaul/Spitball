using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserInviteDto
    {
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public MessageType InviteType { get; set; }
        public string BoxName { get; set; }
        public long BoxId { get; set; }
        public bool Status { get; set; }


        public string BoxPicture { get; set; }

        public string Email { get; set; }
    }
}
