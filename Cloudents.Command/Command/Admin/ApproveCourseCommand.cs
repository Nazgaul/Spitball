using Cloudents.Core.Enum;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveCourseCommand : ICommand
    {
        public ApproveCourseCommand(string course, string subject, SchoolType schoolType)
        {
            Course = course;
            Subject = subject;
            SchoolType = schoolType;
        }
        public string Course { get; }
        public string Subject { get; }
        public SchoolType SchoolType { get; }
    }
}
