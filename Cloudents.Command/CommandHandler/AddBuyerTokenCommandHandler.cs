﻿using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Command.CommandHandler
{
    public class AddBuyerTokenCommandHandler : ICommandHandler<AddBuyerTokenCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public AddBuyerTokenCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(AddBuyerTokenCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var paymePayment = new PaymePayment(message.Token, message.Expiration, message.BuyerCardMask);
            user.AddPayment(paymePayment);
           // user.AddPayment(message.Token, message.Expiration, message.BuyerCardMask);
        }
    }

    public class ConfirmPaymentCommandHandler : ICommandHandler<ConfirmPaymentCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public ConfirmPaymentCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ConfirmPaymentCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.CreditCardReceived();
        }
    }
}