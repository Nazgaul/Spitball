using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class RedeemTokenCommandHandler : ICommandHandler<RedeemTokenCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;



        public RedeemTokenCommandHandler(IRegularUserRepository userRepository, 
            IRepository<Transaction> transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(RedeemTokenCommand message, CancellationToken token)
        {
            var balance = await _userRepository.UserCashableBalanceAsync(message.UserId, token);
            if (balance < message.Amount)
            {
                throw new InvalidOperationException("user doesn't have enough money");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);

            var price = -Math.Abs(message.Amount);
            var t = new Transaction(TransactionActionType.CashOut, TransactionType.Earned, price, user);

            await _transactionRepository.AddAsync(t, token);
            user.Events.Add(new RedeemEvent(message.UserId, message.Amount));
        }
    }
}