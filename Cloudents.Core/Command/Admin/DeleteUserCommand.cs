using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class DeleteUserCommand : ICommand
    {
        public string Email{ get; set; }

    }
}
