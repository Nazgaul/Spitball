using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class CashOutCommandHandler : ICommandHandler<CashOutCommand>
    {
        private readonly IRegularUserRepository _userRepository;



        public CashOutCommandHandler(IRegularUserRepository userRepository
           )
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(CashOutCommand message, CancellationToken token)
        {
            var balance = await _userRepository.UserBalanceAsync(message.UserId, token);
            if (balance < 1000)
            {
                throw new InvalidOperationException("user doesn't have enough money");
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.CashOutMoney(/*message.Amount*/);
            await _userRepository.UpdateAsync(user, token);

        }
    }
}