using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
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