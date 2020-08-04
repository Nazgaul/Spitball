using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class ChatUserDto
    {
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string? Image { get; set; }

      

        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }

    }
}