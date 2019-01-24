﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TransactionEvent : IEvent
    {
        public TransactionEvent(Transaction tx, RegularUser user)
        {
            Transaction = tx;
            User = user;
        }

        public Transaction Transaction { get;  }
        public RegularUser User { get; }
    }
}
