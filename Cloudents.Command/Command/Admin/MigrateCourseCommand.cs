using System;

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

    public class CreateCourseCommand : ICommand
    {
        public CreateCourseCommand(string courseName, long subjectId)
        {
            CourseName = courseName;
            SubjectId = subjectId;
        }

        public string CourseName { get; }
        public long SubjectId { get; }

    }
}