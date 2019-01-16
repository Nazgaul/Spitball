using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class FinishRegistrationCommandHandler : ICommandHandler<FinishRegistrationCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public FinishRegistrationCommandHandler(IRepository<RegularUser> userRepository
            )
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(FinishRegistrationCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            //TODO: move to transaction repository
            user.MakeTransaction(TransactionType2.FinishRegistration(user.Country));
            await _userRepository.UpdateAsync(user, token);
        }


    }
}
