using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand>
    {


        private readonly IUserRepository _userRepository;


        public SuspendUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(SuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.Events.Add(new SuspendUserEvent(user));
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
