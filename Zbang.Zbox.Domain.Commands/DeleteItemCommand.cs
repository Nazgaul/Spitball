using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemCommand : ICommand
    {

        public DeleteItemCommand(long itemId, long userId, long boxId)
        {
            ItemId = itemId;
            UserId = userId;
            BoxId = boxId;
        }

        public long ItemId
        {
            get;
            private set;

        }
        public long BoxId { get; private set; }

        public long UserId { get; private set; }

        
    }
}
