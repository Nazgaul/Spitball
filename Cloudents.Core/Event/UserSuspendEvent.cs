using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UserSuspendEvent : IEvent
    {
        public UserSuspendEvent(User user)
        {
            User = user;
        }
        public User User { get; private set; }
    }
}
