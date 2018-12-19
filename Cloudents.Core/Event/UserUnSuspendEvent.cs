using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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
