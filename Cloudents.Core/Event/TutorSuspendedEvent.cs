using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TutorSuspendedEvent : IEvent
    {
        public TutorSuspendedEvent(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}
