using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Attributes;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Common.Enum;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.CommandHandler.Admin
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
