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
    public class Transaction : BaseDomain
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        public Transaction(ActionType action, TransactionType type, decimal price, User user) : base()
        {
            Action = action;
            Type = type;
            Price = price;
            Created = DateTime.UtcNow;
            User = user;
            Events.Add(new TransactionEvent(this));
        }

        public virtual Guid Id { get; protected set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; protected set; }

        public virtual ActionType Action { get; protected set; }
        public virtual TransactionType Type { get; protected set; }
        public virtual decimal Price { get; protected set; }

        [CanBeNull] public virtual Question Question { get; set; }
        [CanBeNull] public virtual Answer Answer { get;  set; }
        [CanBeNull] public virtual User InvitedUser { get; set; }
    }
}