using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class UpdateUserCommand : ICommand
    {
        public UpdateUserCommand(User user)
        {
            User = user;
        }

        public User User { get; private set; }
    }
}