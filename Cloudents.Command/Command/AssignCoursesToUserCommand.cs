using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class AssignCoursesToUserCommand : ICommand
    {
        public AssignCoursesToUserCommand(IEnumerable<string> name, long userId)
        {
            Name = name;
            UserId = userId;
        }


        public IEnumerable<string> Name { get; }
        public long UserId { get; }
    }
}