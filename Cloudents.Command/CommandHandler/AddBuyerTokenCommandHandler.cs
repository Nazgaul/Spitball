using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

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
            user.BuyerPayment = new BuyerPayment(message.Token, message.Expiration);
        }
    }
}