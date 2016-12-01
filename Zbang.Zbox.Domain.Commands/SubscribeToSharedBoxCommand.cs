using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    public class SubscribeToSharedBoxCommand : ICommand
    {
        public SubscribeToSharedBoxCommand(long userId, long boxId)
        {
            UserId = userId;
            BoxId = boxId;
        }

        public long UserId { get; private set; }

        public long BoxId { get; private set; }


    }
}