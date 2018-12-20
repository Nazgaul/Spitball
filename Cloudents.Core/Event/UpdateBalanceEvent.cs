using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UpdateBalanceEvent : IEvent
    {
        public UpdateBalanceEvent(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}
