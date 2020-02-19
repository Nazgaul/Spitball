using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnSuspendUserCommandHandler : ICommandHandler<UnSuspendUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;


        public UnSuspendUserCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task ExecuteAsync(UnSuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            user.UnSuspendUser();
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
