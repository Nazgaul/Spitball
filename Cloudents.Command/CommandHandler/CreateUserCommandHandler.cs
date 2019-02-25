using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public CreateUserCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            await _userRepository.AddAsync(message.User, token);
        }
    }
}