using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.Message.Email
{
    public class RedeemTransactionMessage : ISystemQueueMessage
    {
        public RedeemTransactionMessage(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")] 
        public Guid TransactionId { get; private set; }
    }
}