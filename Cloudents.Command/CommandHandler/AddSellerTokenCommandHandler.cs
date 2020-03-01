using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddSellerTokenCommandHandler : ICommandHandler<AddSellerTokenCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public AddSellerTokenCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync([NotNull] AddSellerTokenCommand message, CancellationToken token)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(message.Token)) throw new ArgumentNullException(nameof(message));
            var user = await _userRepository.GetUserByEmailAsync(message.UserEmail, token);
            if (user == null)
            {
                throw new NullReferenceException($"{message.UserEmail} does not exists");
            }
            //if (user.Tutor.SellerKey != null)
            //{
            //    throw new ArgumentException();
            //}
            user.Tutor.SellerKey = message.Token;
        }
    }
}