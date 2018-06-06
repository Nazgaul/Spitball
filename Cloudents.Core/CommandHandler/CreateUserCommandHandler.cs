using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CreateUserCommandHandler : ICommandHandlerAsync<CreateUserCommand>

    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(CreateUserCommand message, CancellationToken token)
        {
            await _userRepository.SaveAsync(message.User, token).ConfigureAwait(false);
        }
    }
}