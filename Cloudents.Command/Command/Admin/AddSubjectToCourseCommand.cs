using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class AddSubjectToCourseCommand: ICommand
    {
        public AddSubjectToCourseCommand(string courseName, string subject)
        {
            CourseName = courseName;
            Subject = subject;
        }
        public string CourseName { get; set; }
        public string Subject { get; set; }
    }
}
