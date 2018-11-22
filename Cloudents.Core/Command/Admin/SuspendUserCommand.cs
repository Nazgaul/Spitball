using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class SuspendUserCommand : ICommand
    {
        public SuspendUserCommand(long id, bool shouldDeleteData)
        {
            Id = id;
            ShouldDeleteData = shouldDeleteData;
        }
        //public string Email{ get; set; }

        public long  Id { get; }


        public bool ShouldDeleteData { get; private set; }

    }
}
