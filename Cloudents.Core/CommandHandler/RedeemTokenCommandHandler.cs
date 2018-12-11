using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class RedeemTokenCommandHandler : ICommandHandler<RedeemTokenCommand>
    {
        private readonly IUserRepository _userRepository;

        public RedeemTokenCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(RedeemTokenCommand message, CancellationToken token)
        {
            var balance = await _userRepository.UserCashableBalanceAsync(message.UserId, token);
            if (balance < message.Amount)
            {
                throw new InvalidOperationException("user doesn't have enough money");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (user.Fictive.GetValueOrDefault())
            {
                throw new UnauthorizedAccessException("Fictive user");
            }
            user.AddTransaction(Transaction.CashOut(message.Amount));
            await _userRepository.UpdateAsync(user, token);
            user.Events.Add(new RedeemEvent(message.UserId, message.Amount));
        }
    }
}