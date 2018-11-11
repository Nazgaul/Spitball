using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TransactionReferredEvent : IEvent
    {
        public TransactionReferredEvent(Transaction tx)
        {
            Transaction = tx;
        }

        public Transaction Transaction { get; private set; }
    }
}
