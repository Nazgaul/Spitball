using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class NewCourseEvent : IEvent
    {
        public NewCourseEvent(Course userCourse)
        {
            UserCourse = userCourse;
        }

        public Course UserCourse { get; private set; }
    }
    public class UpdateCourseEvent : IEvent
    {
        public UpdateCourseEvent(Course userCourse)
        {
            UserCourse = userCourse;
        }

        public Course UserCourse { get; private set; }
    }

   
}
