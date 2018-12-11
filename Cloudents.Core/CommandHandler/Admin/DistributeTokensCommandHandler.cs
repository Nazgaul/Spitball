using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    [AdminCommandHandler]
    public class DistributeTokensCommandHandler : ICommandHandler<DistributeTokensCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;

        public DistributeTokensCommandHandler(IRepository<RegularUser> userRepository, IRepository<Transaction> transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }


        public async Task ExecuteAsync(DistributeTokensCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var transaction = new Transaction(message.ActionType, TransactionType.Earned, message.Price, user);
            await _transactionRepository.AddAsync(transaction, token);
        }
    }
}
