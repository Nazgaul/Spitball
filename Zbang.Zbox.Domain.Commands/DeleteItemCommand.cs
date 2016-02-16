using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemCommand : ICommandAsync
    {

        public DeleteItemCommand(long itemId, long userId)
        {
            ItemId = itemId;
            UserId = userId;
        }

        public long ItemId
        {
            get;
            private set;

        }

        public long UserId { get; private set; }

        
    }
}
