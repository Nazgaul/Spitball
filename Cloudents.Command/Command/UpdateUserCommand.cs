using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(RegularUser user)
        {
            User = user;
        }

        public RegularUser User { get; private set; }
    }
}