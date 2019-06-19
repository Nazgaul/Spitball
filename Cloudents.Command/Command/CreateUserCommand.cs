using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class CreateUserCommand : ICommand
    {
        public CreateUserCommand(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}