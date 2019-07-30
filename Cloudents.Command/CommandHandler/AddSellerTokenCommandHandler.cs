using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class AddSellerTokenCommandHandler : ICommandHandler<AddSellerTokenCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public AddSellerTokenCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(AddSellerTokenCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetUserByEmailAsync(message.UserEmail, token);
            if (user == null)
            {
                throw new NullReferenceException($"{message.UserEmail} does not exists");
            }
            if (user.Tutor.SellerKey != null)
            {
                throw new ArgumentException();
            }
            user.Tutor.SellerKey = message.Token;
        }
    }
}