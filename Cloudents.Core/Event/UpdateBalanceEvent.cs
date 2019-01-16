using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UpdateBalanceEvent : IEvent
    {
        public UpdateBalanceEvent(RegularUser user)
        {
            User = user;
        }

        public RegularUser User { get; set; }
    }
}
