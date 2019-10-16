using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class EndStudyRoomSessionEvent : IEvent
    {
        public EndStudyRoomSessionEvent(StudyRoomSession session)
        {
            Session = session;
        }
        public StudyRoomSession Session { get; private set; }
    }
}
