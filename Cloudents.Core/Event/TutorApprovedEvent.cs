using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TutorApprovedEvent : IEvent
    {
        public TutorApprovedEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
