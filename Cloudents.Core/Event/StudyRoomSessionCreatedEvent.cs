using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class StudyRoomSessionCreatedEvent : IEvent
    {
        public StudyRoomSession StudyRoomSession { get; }

        public StudyRoomSessionCreatedEvent(StudyRoomSession studyRoomSession)
        {
            StudyRoomSession = studyRoomSession;
        }
    }


    public class StudyRoomSessionRejoinEvent : IEvent
    {
        public StudyRoomSession StudyRoomSession { get; }

        public StudyRoomSessionRejoinEvent(StudyRoomSession studyRoomSession)
        {
            StudyRoomSession = studyRoomSession;
        }
    }
}