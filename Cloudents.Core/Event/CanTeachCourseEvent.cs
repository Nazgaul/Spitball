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

    //public class CourseChangeSubjectEvent : IEvent
    //{
    //    public CourseChangeSubjectEvent(Course course)
    //    {
    //        Course = course;
    //    }

    //    public Course Course { get;  }
    //}

    //public class RemoveCourseEvent : IEvent
    //{
    //    public RemoveCourseEvent(long userId)
    //    {
    //        UserId = userId;
    //    }

    //    public long UserId { get; private set; }
    //}
}
