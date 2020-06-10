using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class SubscribeToTutorEvent : IEvent
    {
        private User user;

        public SubscribeToTutorEvent(User user)
        {
            this.user = user;
        }
    }
}