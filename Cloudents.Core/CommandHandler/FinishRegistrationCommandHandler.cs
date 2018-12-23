using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Common.Enum;
using Cloudents.Core.Enum;

namespace Cloudents.Core.CommandHandler
{
    public class FinishRegistrationCommandHandler : ICommandHandler<FinishRegistrationCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;

        public FinishRegistrationCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Transaction> transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(FinishRegistrationCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            //TODO: move to transaction repository
            if (user.Transactions.All(a => a.Action != TransactionActionType.SignUp))
            {
                var balance = ReputationAction.FinishRegister(user.Country);
                var t = new Transaction(TransactionActionType.SignUp, TransactionType.Earned, balance, user);
                await _transactionRepository.AddAsync(t, token);
            }
        }


    }
}
