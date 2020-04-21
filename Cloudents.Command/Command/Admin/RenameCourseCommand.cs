namespace Cloudents.Command.Command.Admin
{
    public class RenameCourseCommand : ICommand
    {
        public RenameCourseCommand(string courseName, string newName)
        {
            CourseName = courseName;
            NewName = newName;
        }
        public string CourseName { get;  }
        public string NewName { get;  }
    }
}