namespace Cloudents.Command.Command.Admin
{
    public class MigrateCourseCommand : ICommand
    {
        public MigrateCourseCommand(string courseToKeep, string courseToRemove)
        {
            CourseToKeep = courseToKeep;
            CourseToRemove = courseToRemove;
        }
        public string CourseToKeep { get;  }
        public string CourseToRemove { get; }
    }
}