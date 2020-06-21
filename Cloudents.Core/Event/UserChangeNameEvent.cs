using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UserChangeNameEvent : IEvent
    {
        public UserChangeNameEvent(User user)
        {
            User = user;
        }

        public User User { get;  }
    }
}