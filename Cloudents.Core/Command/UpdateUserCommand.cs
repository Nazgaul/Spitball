using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Command
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