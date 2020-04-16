using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class CanTeachCourseEvent : IEvent
    {
        public CanTeachCourseEvent(UserCourse2 userCourse)
        {
            UserCourse = userCourse;
        }

        public UserCourse2 UserCourse { get; private set; }
    }

    public class RemoveCourseEvent : IEvent
    {
        public RemoveCourseEvent(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
