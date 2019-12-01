using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TutorUnSuspendedEvent : IEvent
    {
        public TutorUnSuspendedEvent(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}
