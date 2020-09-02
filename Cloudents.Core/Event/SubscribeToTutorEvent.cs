using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class SubscribeToTutorEvent : IEvent
    {
        public Follow Follow { get; }

        public SubscribeToTutorEvent(Follow follow)
        {
            Follow = follow;
        }
    }
}