using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    public class Transaction
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        public Transaction( ActionType action, TransactionType type, decimal price)
        {
            //User = user;
            Action = action;
            Type = type;
            Price = price;
            //Balance = balance;
            if (Balance < 0)
            {
                throw new InvalidOperationException("not enough tokens");
            }
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