using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UserUnSuspendEvent : IEvent
    {
        public UserUnSuspendEvent(RegularUser user)
        {
            User = user;
        }
        public RegularUser User { get; private set; }
    }
}
