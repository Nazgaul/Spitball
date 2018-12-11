using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
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
