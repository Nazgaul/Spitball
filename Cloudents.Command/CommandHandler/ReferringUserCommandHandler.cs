using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ReferringUserCommandHandler : ICommandHandler<ReferringUserCommand>
    {
        private readonly IRepository<User> _userRepository;
      

        //private const int MaxRefer = 5;

        public ReferringUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
           
        }

        public async Task ExecuteAsync(ReferringUserCommand message, CancellationToken token)
        {
            if (message.InvitingUserId == message.RegisteredUserId)
            {
                return;
            }
            var user = await _userRepository.LoadAsync(message.InvitingUserId, token);
            var register = await _userRepository.LoadAsync(message.RegisteredUserId, token);
            user.ReferUser(register);
            await _userRepository.UpdateAsync(user, token);

        }
    }
}