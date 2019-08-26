using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TutorAddReviewEvent : IEvent
    {
        public TutorAddReviewEvent(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}