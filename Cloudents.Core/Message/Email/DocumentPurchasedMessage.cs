using Cloudents.Core.Message.System;
using System;

namespace Cloudents.Core.Message.Email
{
    public class DocumentPurchasedMessage : ISystemQueueMessage
    {
        public DocumentPurchasedMessage(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        public Guid TransactionId { get; private set; }
    }
}