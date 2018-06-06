using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateUserCommand : ICommand
    {
        public CreateUserCommand(User user)
        {
            User = user;
        }

        public User User { get; private set; }
    }
}