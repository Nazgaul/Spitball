using Cloudents.Core.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class UnSuspendUserCommandHandler : ICommandHandler<UnSuspendUserCommand>
    {
        private readonly IUserRepository _userRepository;


        public UnSuspendUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task ExecuteAsync(UnSuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            if (user.Fictive.GetValueOrDefault())
            {
                return;
            }

            user.LockoutEnd = null;
            user.Events.Add(new UserUnSuspendEvent(user));
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
