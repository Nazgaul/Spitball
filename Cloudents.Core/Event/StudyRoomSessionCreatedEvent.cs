using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class StudyRoomSessionCreatedEvent : IEvent
    {
        public StudyRoomSession StudyRoomSession { get; }

        public StudyRoomSessionCreatedEvent(StudyRoomSession studyRoomSession)
        {
            this.StudyRoomSession = studyRoomSession;
        }
    }
}