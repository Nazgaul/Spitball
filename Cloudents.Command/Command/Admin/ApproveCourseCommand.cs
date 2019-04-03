namespace Cloudents.Command.Command.Admin
{
    public class ApproveCourseCommand: ICommand
    {
        public ApproveCourseCommand(string course)
        {
            Course = course;
        }
        public string Course { get; set; }
    }
}
