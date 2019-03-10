using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class DeleteCourseCommand : ICommand
    {
        public DeleteCourseCommand( string courseToRemove)
        {
            CourseToRemove = courseToRemove;
        }
        public string CourseToRemove { get; set; }
    }
}
