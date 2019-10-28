using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TutorDeletedEvent : IEvent
    {
        public TutorDeletedEvent(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}
