using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Core.Command
{
    public class CreateUserCommand : ICommand
    {
        public CreateUserCommand(RegularUser user)
        {
            User = user;
        }

        public RegularUser User { get; private set; }
    }
}