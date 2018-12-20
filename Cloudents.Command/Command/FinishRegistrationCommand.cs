using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
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
