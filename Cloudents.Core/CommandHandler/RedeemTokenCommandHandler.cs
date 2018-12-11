using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class RedeemTokenCommandHandler : ICommandHandler<RedeemTokenCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;

        private readonly IQueueProvider _serviceBusProvider;

        public RedeemTokenCommandHandler(IRegularUserRepository userRepository, IQueueProvider serviceBusProvider, IRepository<Transaction> transactionRepository)
        {
            _userRepository = userRepository;
            _serviceBusProvider = serviceBusProvider;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(RedeemTokenCommand message, CancellationToken token)
        {
            var balance = await _userRepository.UserEarnedBalanceAsync(message.UserId, token);
            if (balance < message.Amount)
            {
                throw new InvalidOperationException("user doesn't have enough money");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);

            var price = -Math.Abs(message.Amount);
            var t = new Transaction(ActionType.CashOut, TransactionType.Earned, price, user);

            await _transactionRepository.AddAsync(t, token);
            //TODO: need to be in event
            await _serviceBusProvider.InsertMessageAsync(new SupportRedeemEmail(message.Amount, user.Id), token);
        }
    }
}