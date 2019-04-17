using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class StudyRoomCreatedEvent : IEvent
    {
        public StudyRoomCreatedEvent(StudyRoom studyRoom)
        {
            StudyRoom = studyRoom;
        }

        public StudyRoom StudyRoom { get;  }

    }


    public class StudyRoomOnlineChangeEvent : IEvent
    {
        public StudyRoomOnlineChangeEvent(StudyRoomUser studyUser)
        {
            StudyUser = studyUser;
        }

        public StudyRoomUser StudyUser { get; }
    }
}