using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UserUnSuspendEvent : IEvent
    {
        public UserUnSuspendEvent(User user)
        {
            User = user;
        }
        public User User { get; private set; }
    }
}
