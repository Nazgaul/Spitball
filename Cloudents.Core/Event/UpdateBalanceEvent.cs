using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;

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
