using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class FinishRegistrationCommand : ICommand
    {
        public FinishRegistrationCommand(long id)
        {
            Id = id;
        }
        public long Id { get; set; }
    }
}
