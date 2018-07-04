using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Core.CommandHandler
{
    public class RedeemTokenCommandHandler : ICommandHandler<RedeemTokenCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceBusProvider _serviceBusProvider;

        public RedeemTokenCommandHandler(IUserRepository userRepository, IServiceBusProvider serviceBusProvider)
        {
            _userRepository = userRepository;
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task ExecuteAsync(RedeemTokenCommand message, CancellationToken token)
        {
            var balance = await _userRepository.UserEarnedBalanceAsync(message.UserId, token);
            if (balance < message.Amount)
            {
                throw new InvalidOperationException("user doesn't have enough money");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.AddTransaction(new Transaction(ActionType.CashOut, TransactionType.Earned, -message.Amount));
            await _userRepository.UpdateAsync(user, token);
            await _serviceBusProvider.InsertMessageAsync(new SupportRedeemEmail(message.Amount, user.Id), token);
        }
    }
}