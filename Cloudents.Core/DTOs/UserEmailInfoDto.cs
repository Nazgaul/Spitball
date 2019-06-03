using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class UserEmailInfoDto
    {
        [EntityBind(nameof(RegularUser.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(RegularUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(Core.Entities.University.Name))]
        public string University { get; set; }
        [EntityBind(nameof(RegularUser.Country))]
        public string Country { get; set; }

        [EntityBind(nameof(RegularUser.PhoneNumber))]
        public string PhoneNumber { get; set; }
    }
}
