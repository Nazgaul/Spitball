using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class DocumentCreatedEvent : IEvent
    {
        public DocumentCreatedEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }

    public class CourseEnrollmentEvent : IEvent
    {
        public CourseEnrollmentEvent(CourseEnrollment enrollment)
        {
            Enrollment = enrollment;
        }

        public CourseEnrollment Enrollment { get;  }
    }
}