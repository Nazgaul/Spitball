using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Common.Enum;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class RedeemTokenCommandHandler : ICommandHandler<RedeemTokenCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IEventStore _eventStore;



        public RedeemTokenCommandHandler(IRegularUserRepository userRepository, 
            IRepository<Transaction> transactionRepository, IEventStore eventStore)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _eventStore = eventStore;
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
            _eventStore.Add(new RedeemEvent(message.UserId, message.Amount));
        }
    }
}