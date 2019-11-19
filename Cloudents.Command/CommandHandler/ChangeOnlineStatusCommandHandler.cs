using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ChangeOnlineStatusCommandHandler : ICommandHandler<ChangeOnlineStatusCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public ChangeOnlineStatusCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ChangeOnlineStatusCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.ChangeOnlineStatus(message.Status);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}