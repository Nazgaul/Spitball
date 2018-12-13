using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;

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