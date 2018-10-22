using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class AssignCourseToUserCommand : ICommand
    {
        public AssignCourseToUserCommand(long userId, long courseId)
        {
            UserId = userId;
            CourseId = courseId;
        }

        public long UserId { get; private set; }
        public long CourseId { get; private set; }
    }
}