using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UnFollowBoxCommand: ICommand
    {
        public UnFollowBoxCommand(long boxId, long userId)
        {
            BoxId = boxId;
            UserId = userId;
        }
        public long BoxId { get;private set; }

        public long UserId { get;private set; }
    }
}
