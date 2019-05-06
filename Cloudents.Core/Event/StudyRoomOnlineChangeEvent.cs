using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class StudyRoomOnlineChangeEvent : IEvent
    {
        public StudyRoomOnlineChangeEvent(StudyRoomUser studyUser)
        {
            StudyUser = studyUser;
        }

        public StudyRoomUser StudyUser { get; }
    }


    public class StudentPaymentReceivedEvent : IEvent
    {
        public StudentPaymentReceivedEvent(RegularUser user)
        {
            User = user;
        }

        public RegularUser User { get; }
    }
}