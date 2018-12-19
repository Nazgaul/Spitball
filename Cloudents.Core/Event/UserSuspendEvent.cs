using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
{
    public class UserSuspendEvent : IEvent
    {
        public UserSuspendEvent(RegularUser user)
        {
            User = user;
        }
        public RegularUser User { get; private set; }
    }
}
