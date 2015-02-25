using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteBoxCommand : ICommandAsync
    {
        public DeleteBoxCommand(long boxId)
        {
            BoxId = boxId;
        }

        public long BoxId { get;private set; }
    }
}
