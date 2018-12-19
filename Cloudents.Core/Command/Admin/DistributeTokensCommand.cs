using Cloudents.Application.Interfaces;
using Cloudents.Common.Enum;

namespace Cloudents.Application.Command.Admin
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
