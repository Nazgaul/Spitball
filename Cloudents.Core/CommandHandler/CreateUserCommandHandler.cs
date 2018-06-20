using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>

    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            await _userRepository.AddOrUpdateAsync(message.User, token).ConfigureAwait(false);
        }
    }


    
}