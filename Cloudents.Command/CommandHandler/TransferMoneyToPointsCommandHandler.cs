﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class TransferMoneyToPointsCommandHandler : ICommandHandler<TransferMoneyToPointsCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public TransferMoneyToPointsCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(TransferMoneyToPointsCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var t = new BuyPointsTransaction(message.Amount, message.PayPalTransactionId);
            user.MakeTransaction(t);

            await _userRepository.UpdateAsync(user, token);
        }
    }
}