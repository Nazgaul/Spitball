using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ReferringUserCommandHandler : ICommandHandler<ReferringUserCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IReferUserTransactionRepository _referUserTransactionRepository;

        private const int MaxRefer = 5;

        public ReferringUserCommandHandler(IRepository<User> userRepository, IReferUserTransactionRepository referUserTransactionRepository)
        {
            _userRepository = userRepository;
            _referUserTransactionRepository = referUserTransactionRepository;
        }

        public async Task ExecuteAsync(ReferringUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.InvitingUserId, token);
            var referCount = await _referUserTransactionRepository.GetReferUserCountAsync(user.Id, token);

            if (user.Id != message.RegisteredUserId && 
                user.Country != Country.India.Name &&
                referCount < MaxRefer)
            {
                var register = await _userRepository.LoadAsync(message.RegisteredUserId, token);
                user.ReferUser(register);
                await _userRepository.UpdateAsync(user, token);
            }
         
        }
    }
}