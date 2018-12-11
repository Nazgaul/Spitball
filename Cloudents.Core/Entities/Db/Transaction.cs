using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Exceptions;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate Proxy")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nHibernate Proxy")]
    public class Transaction : DomainObject
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        public Transaction(TransactionActionType action, TransactionType type, decimal price, RegularUser user) : base()
        {
            Action = action;
            Type = type;
            Price = price;
            Created = DateTime.UtcNow;
            User = user;
            Events.Add(new TransactionEvent(this));
            //}
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

        [CanBeNull] public virtual Question Question { get; set; }
        [CanBeNull] public virtual Answer Answer { get;  set; }
        [CanBeNull] public virtual RegularUser InvitedUser { get; set; }
    }
}