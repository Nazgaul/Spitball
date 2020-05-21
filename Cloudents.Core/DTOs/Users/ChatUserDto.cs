using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class ChatUserDto
    {
        [EntityBind(nameof(BaseUser.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(BaseUser.ImageName))]
        public string? Image { get; set; }

        [EntityBind(nameof(ChatUser.Unread))]
        public int Unread { get; set; }

        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }

    }
}