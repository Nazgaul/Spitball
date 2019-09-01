using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UpdateImageEvent : IEvent
    {
        public UpdateImageEvent(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
