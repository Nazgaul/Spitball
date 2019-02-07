using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveCashOutCommandHandler : ICommandHandler<ApproveCashOutCommand>
    {
        private readonly IRepository<CashOutTransaction> _transactionRepository;

        public ApproveCashOutCommandHandler(IRepository<CashOutTransaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(ApproveCashOutCommand message, CancellationToken token)
        {
            var tran = await _transactionRepository.LoadAsync(message.TransactionId, token);
            tran.Approve();
        }
    }
}
