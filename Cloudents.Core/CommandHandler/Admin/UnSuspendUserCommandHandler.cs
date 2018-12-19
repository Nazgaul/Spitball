using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.CommandHandler.Admin
{
    public class UnSuspendUserCommandHandler : ICommandHandler<UnSuspendUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IEventStore _eventStore;


        public UnSuspendUserCommandHandler(IRegularUserRepository userRepository, IEventStore eventStore)
        {
            _userRepository = userRepository;
            _eventStore = eventStore;
        }
        public async Task ExecuteAsync(UnSuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            user.LockoutEnd = null;
            _eventStore.Add(new UserUnSuspendEvent(user));
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
