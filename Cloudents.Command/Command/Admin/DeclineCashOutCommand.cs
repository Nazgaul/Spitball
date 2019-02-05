using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class DeclineCashOutCommand : ICommand
    {
        public DeclineCashOutCommand(Guid transactionId, string reason)
        {
            TransactionId = transactionId;
            Reason = reason;
        }
        public Guid TransactionId { get; set; }
        public string Reason { get; set; }
    }
}
