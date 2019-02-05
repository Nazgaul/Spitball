using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeclineCashOutCommandHandler : ICommandHandler<DeclineCashOutCommand>
    {
        private readonly IRepository<CashOutTransaction> _transactionRepository;

        public DeclineCashOutCommandHandler(IRepository<CashOutTransaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(DeclineCashOutCommand message, CancellationToken token)
        {
            var tran = await _transactionRepository.LoadAsync(message.TransactionId, token);
            tran.Decline(message.Reason);
        }
    }
}
