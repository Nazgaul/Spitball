
using System;

namespace Cloudents.Core.DTOs.Tutors
{
    public class TutorActionsDto
    {

        public bool PhoneVerified { get; set; }
        public bool EmailVerified { get; set; }
        public bool EditProfile { get; set; }
        public BookedSession? BookedSession { get; set; }
        public bool Courses { get; set; }

        public bool StripeAccount { get; set; }
        public bool CalendarShared { get; set; }
        public bool HaveHours { get; set; }

        public bool LiveSession { get; set; }

        public bool UploadContent { get; set; }
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
