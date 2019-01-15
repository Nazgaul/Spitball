using Cloudents.Core.Enum;
using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate Proxy")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nHibernate Proxy")]
    public class Transaction : AggregateRoot
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        public Transaction(TransactionActionType action, TransactionType type, decimal price, RegularUser user)
        {
            Action = action;
            Type = type;
            Price = price;
            Created = DateTime.UtcNow;
            User = user;
            AddEvent(new TransactionEvent(this));

        }

        protected Transaction()
        {

        }

        public virtual Guid Id { get; protected set; }
        public virtual RegularUser User { get; set; }

        public virtual DateTime Created { get; protected set; }

        public virtual TransactionActionType Action { get; protected set; }
        public virtual TransactionType Type { get; protected set; }
        public virtual decimal Price { get; protected set; }

        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual RegularUser InvitedUser { get; set; }

        public virtual Document Document { get; set; }

      
    }
}