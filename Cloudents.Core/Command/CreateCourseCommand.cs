
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

        public long Id { get; set; }
    }
}