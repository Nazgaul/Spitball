using System;

namespace Cloudents.Command.Command.Admin
{
    public class DeclineCashOutCommand : ICommand
    {
        public DeclineCashOutCommand(Guid transactionId, string reason)
        {
            TransactionId = transactionId;
            Reason = reason;
        }
        public Guid TransactionId { get; }
        public string Reason { get; }
    }
}
