
using System;

namespace Cloudents.Core.DTOs.Tutors
{
    public class TutorActionsDto
    {

        public bool PhoneVerified { get; set; }
        public bool EmailVerified { get; set; }
        public bool EditProfile { get; set; }
        public BookedSession? BookedSession { get; set; }

        public bool StripeAccount { get; set; }
        public bool CalendarShared { get; set; }
        public bool LiveSession { get; set; }

    }

    public class TutorNotificationDto
    {
        public int PendingPayment { get; set; }
        public int UnreadChatMessages { get; set; }
        public int UnansweredQuestion { get; set; }
        public int LiveClassRegisteredUser { get; set; }
        public int FollowerNoCommunication { get; set; }
    }

    public class UpcomingStudyRoomDto
    {
        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public int StudentEnroll { get; set; }

        public Guid StudyRoomId { get; set; }

        public string StudyRoomName { get; set; }

        public DateTime BroadcastTime { get; set; }
    }

    public class BookedSession
    {
        [NonSerialized] public long? _TutorId;
        public bool Exists { get; set; }

        public long? TutorId
        {
            get
            {
                if (!Exists)
                {
                    return _TutorId ?? 0;
                }

                return null;
            }
        }
    }


}
