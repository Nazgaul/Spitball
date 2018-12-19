using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Command
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