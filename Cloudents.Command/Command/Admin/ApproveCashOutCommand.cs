using System;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveCashOutCommand : ICommand
    {
        public ApproveCashOutCommand(Guid transactionId)
        {
            TransactionId = transactionId;
        }
        public Guid TransactionId { get; }
    }
}
