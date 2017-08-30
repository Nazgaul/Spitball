using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UnFollowBoxCommand: ICommandAsync
    {
        public UnFollowBoxCommand(long boxId, long userId, bool shouldDelete)
        {
            ShouldDelete = shouldDelete;
            BoxId = boxId;
            UserId = userId;
        }
        public long BoxId { get;private set; }

        public long UserId { get;private set; }

        public bool ShouldDelete { get; private set; }
    }
}
