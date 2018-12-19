using System.Collections.Generic;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
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