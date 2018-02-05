using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateCourseCommand : ICommand
    {
        public CreateCourseCommand(string name, long universityId)
        {
            Name = name;
            UniversityId = universityId;
        }


        public string Name { get; private set; }

        public long UniversityId { get; private set; }

    }

    public class CreateCourseCommandResult : ICommandResult
    {
        public CreateCourseCommandResult(long courseId)
        {
            CourseId = courseId;
        }

        public long CourseId { get; private set; }
    }
}
