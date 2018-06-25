using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    public class Transaction
    {
        private Transaction(User user, ActionType action, TransactionType type, decimal price, decimal balance)
        {
            User = user;
            Action = action;
            Type = type;
            Price = price;
            Balance = balance;
            if (Balance < 0)
            {
                throw new InvalidOperationException("not enough tokens");
            }
            Created = DateTime.UtcNow;
        }

        public static Transaction CreateRoot(User user, ActionType action, TransactionType type, decimal price)
        {
            return new Transaction(user, action, type, price, price);
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

        public virtual Transaction NextTransaction { get; set; }

        public virtual Transaction AddTransaction(ActionType action, TransactionType type, decimal price)
        {
            var balance = Balance + Price;
            //if (balance < 0)
            //{
            //    throw new InvalidOperationException("not enough tokens");
            //}
            var t = new Transaction(User, action, type, price, balance);
            NextTransaction = t;
            return NextTransaction;
        }
    }
}