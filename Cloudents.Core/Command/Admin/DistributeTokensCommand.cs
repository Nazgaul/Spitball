using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class DistributeTokensCommand : ICommand
    {
        public DistributeTokensCommand(long userId, decimal price, TransactionActionType actionType)
        {
            UserId = userId;
            Price = price;
            ActionType = actionType;
        }

        public long UserId { get; }
        public decimal Price { get; }
       

        public TransactionActionType ActionType { get; }
    }
}
