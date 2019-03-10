using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class MigrateCourseCommand : ICommand
    {
        public MigrateCourseCommand(string courseToKeep, string courseToRemove)
        {
            CourseToKeep = courseToKeep;
            CourseToRemove = courseToRemove;
        }
        public string CourseToKeep { get; set; }
        public string CourseToRemove { get; set; }
    }
}
