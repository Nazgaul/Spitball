using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    public class Transaction
    {
        public Transaction(User user, ActionType action, TransactionType type, decimal price/*, decimal oldBalance*/)
        {
            User = user;
            Action = action;
            Type = type;
            Price = price;
            Balance = User.Balance + Price;
            Created = DateTime.UtcNow;
        }

        protected Transaction()
        {

        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual ActionType Action { get; set; }
        public virtual TransactionType Type { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal Balance { get; set; }
    }
}