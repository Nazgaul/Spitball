using Cloudents.Core.Message.System;
using System;
using System.Diagnostics.CodeAnalysis;

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