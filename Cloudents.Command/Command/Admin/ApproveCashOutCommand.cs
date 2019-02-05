using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveCashOutCommand : ICommand
    {
        public ApproveCashOutCommand(Guid transactionId)
        {
            TransactionId = transactionId;
        }
        public Guid TransactionId { get; set; }
    }
}
