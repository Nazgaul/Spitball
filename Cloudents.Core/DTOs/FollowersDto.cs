using Cloudents.Core.Attributes;
using System;

namespace Cloudents.Core.DTOs
{
    public class FollowersDto
    {
        [EntityBind(nameof(Entities.User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(Entities.User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(Entities.User.ImageName))]
        public string Image { get; set; }
        [EntityBind(nameof(Entities.Follow.Created))]
        public DateTime Created { get; set; }
        [EntityBind(nameof(Entities.User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(Entities.User.PhoneNumber))]
        public string PhoneNumber { get; set; }
    }
}
