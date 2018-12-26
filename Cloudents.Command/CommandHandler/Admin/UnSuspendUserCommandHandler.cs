using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

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
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            user.LockoutEnd = null;
            user.Events.Add(new UserUnSuspendEvent(user));
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
