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


        public string Name { get; }

        public long UniversityId { get; }
    }

    public class CreateCourseCommandResult : ICommandResult
    {
        public CreateCourseCommandResult(long courseId)
        {
            Id = courseId;
        }

        public long Id { get; }
    }
}
