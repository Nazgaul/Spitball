using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class TransactionEvent : IEvent
    {
        public TransactionEvent(Transaction tx)
        {
            Transaction = tx;
        }

        public Transaction Transaction { get; private set; }
    }
}
