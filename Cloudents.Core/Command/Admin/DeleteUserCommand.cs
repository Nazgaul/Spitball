using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class DeleteUserCommand : ICommand
    {
        public DeleteUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
