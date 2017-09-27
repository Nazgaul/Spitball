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

        public long BoxId { get; }

        public long UserId { get; }

        public bool ShouldDelete { get; }
    }
}
