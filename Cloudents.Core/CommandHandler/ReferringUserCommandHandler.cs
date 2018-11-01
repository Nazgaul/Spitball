using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class ReferringUserCommandHandler : ICommandHandler<ReferringUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public ReferringUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ReferringUserCommand message, CancellationToken token)
        {
            
            var user = await _userRepository.LoadAsync(message.InvitingUserId, token);
            if (user == null)
            {
                //User not exists not crashing the system.
                return;
            }

            var transaction = Transaction.ReferringUserTransaction(user);
            user.AddTransaction(transaction);
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}