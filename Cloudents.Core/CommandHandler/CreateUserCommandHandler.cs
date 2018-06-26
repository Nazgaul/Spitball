using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;


        private const decimal InitialBalance = 100;

        public CreateUserCommandHandler(IUserRepository userRepository, IRepository<Transaction> transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            //var (privateKey, _) = _blockChainErc20Service.CreateAccount();
            var rootTransaction = Transaction.CreateRoot(message.User, ActionType.SignUp,
                TransactionType.Awarded, InitialBalance);
            await _transactionRepository.AddAsync(rootTransaction,token);
            await _userRepository.AddAsync(message.User, token).ConfigureAwait(false);
        }
    }
}