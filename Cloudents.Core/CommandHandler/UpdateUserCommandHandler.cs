using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserCommand message, CancellationToken token)
        {
            await _userRepository.UpdateAsync(message.User, token).ConfigureAwait(false);
        }
    }
}