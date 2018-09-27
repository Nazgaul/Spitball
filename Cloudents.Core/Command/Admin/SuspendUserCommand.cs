using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class SuspendUserCommand : ICommand
    {
        public SuspendUserCommand(long id)
        {
            Id = id;
        }
        //public string Email{ get; set; }

        public long  Id { get; }

    }
}
