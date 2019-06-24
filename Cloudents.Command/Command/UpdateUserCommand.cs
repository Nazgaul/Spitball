using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}