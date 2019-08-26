using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class EndSessionEvent : IEvent
    {
        public EndSessionEvent(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
