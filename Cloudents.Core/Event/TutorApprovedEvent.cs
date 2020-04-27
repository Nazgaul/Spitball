using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TutorApprovedEvent : IEvent
    {
        public TutorApprovedEvent(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; private set; }
    }

    public class TutorCreatedEvent : IEvent
    {
        public TutorCreatedEvent(Tutor tutor)
        {
            Tutor = tutor;
        }

        public Tutor Tutor { get; private set; }
    }
}
