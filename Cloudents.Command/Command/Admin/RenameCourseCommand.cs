using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class RenameCourseCommand : ICommand
    {
        public RenameCourseCommand(string courseName, string newName)
        {
            CourseName = courseName;
            NewName = newName;
        }
        public string CourseName { get; set; }
        public string NewName { get; set; }
    }
}
