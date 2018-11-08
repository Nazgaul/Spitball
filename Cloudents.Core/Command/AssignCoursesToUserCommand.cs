using System.Collections.Generic;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
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