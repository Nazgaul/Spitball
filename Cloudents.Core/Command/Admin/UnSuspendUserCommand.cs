using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class UnSuspendUserCommand : ICommand
    {
        public UnSuspendUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
            
    }
}
