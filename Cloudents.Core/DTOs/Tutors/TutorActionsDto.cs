
namespace Cloudents.Core.DTOs.Tutors
{
    public class TutorActionsDto
    {
        public bool PhoneVerified { get; set; }
        public bool EmailVerified { get; set; }
        public bool EditProfile { get; set; }
        public bool BookedSession { get; set; }
        public bool Courses { get; set; }

        public bool StripeAccount { get; set; }
        public bool CalendarShared { get; set; }
        public bool HaveHours { get; set; }

        public bool LiveSession { get; set; }

        public bool UploadContent { get; set; }
    }
}
