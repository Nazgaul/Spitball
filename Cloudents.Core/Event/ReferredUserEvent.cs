using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Event
{
    class ReferredUserEvent : IEvent
    {
        public ReferredUserEvent(Transaction tx)
        {
            Transaction = tx;
        }

        public Transaction Transaction { get; private set; }
    }
}
