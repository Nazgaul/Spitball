using Cloudents.Core.Enum;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveCourseCommand : ICommand
    {
        public ApproveCourseCommand(string course, string subject)
        {
            Course = course;
            Subject = subject;
        }
        public string Course { get; }
        public string Subject { get; }
    }
}
