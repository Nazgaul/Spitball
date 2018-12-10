using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.CommandHandler
{
    public class FinishRegistrationCommandHandler : ICommandHandler<FinishRegistrationCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;

        public FinishRegistrationCommandHandler(IUserRepository userRepository,
            IRepository<Transaction> transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(FinishRegistrationCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            //TODO: move to transaction repository
            if (user.Transactions.All(a => a.Action != ActionType.SignUp))
            {
                var balance = ReputationSystem.InitBalance(user.Country);
                var t = new Transaction(ActionType.SignUp, TransactionType.Earned, balance, user);
                await _transactionRepository.AddAsync(t, token);
            }
        }


    }
}
