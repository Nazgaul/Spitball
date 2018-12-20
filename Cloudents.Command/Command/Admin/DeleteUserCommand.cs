using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
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
