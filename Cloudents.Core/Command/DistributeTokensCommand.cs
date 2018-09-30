using System;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;


namespace Cloudents.Core.Command
{
    public class DistributeTokensCommand : ICommand
    {
        public DistributeTokensCommand(long userId, decimal price, ActionType actionType, TransactionType transactionType)
        {
            UserId = userId;
            Price = price;
            ActionType = actionType;
            TransactionType = transactionType;
        }

        public long UserId { get; }
        public decimal Price { get; }
        public TransactionType TransactionType { get;  }
       

        public ActionType ActionType { get; }
    }
}
