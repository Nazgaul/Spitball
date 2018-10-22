using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class AssignCourseToUserCommand : ICommand
    {
        public AssignCourseToUserCommand(string name, long userId)
        {
            Name = name;
            UserId = userId;
        }


        public string Name { get; }
        public long UserId { get; }
    }
}