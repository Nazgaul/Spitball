using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
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