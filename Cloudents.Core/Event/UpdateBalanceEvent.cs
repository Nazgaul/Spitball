using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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
