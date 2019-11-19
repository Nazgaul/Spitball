namespace Cloudents.Command.Command.Admin
{
    public class DeleteCourseCommand : ICommand
    {
        public DeleteCourseCommand(string courseToRemove)
        {
            CourseToRemove = courseToRemove;
        }
        public string CourseToRemove { get; }
    }
}
