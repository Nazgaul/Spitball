using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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
