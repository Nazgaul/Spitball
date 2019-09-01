using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class CanTeachCourseEvent : IEvent
    {
        public CanTeachCourseEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
