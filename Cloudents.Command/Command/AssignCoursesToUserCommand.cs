using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class AssignCoursesToUserCommand : ICommand
    {
        public AssignCoursesToUserCommand(IEnumerable<string> names, long userId)
        {
            Names = names;
            UserId = userId;
        }


        public IEnumerable<string> Names { get; }
        public long UserId { get; }
    }
}