using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            if (user.Transactions.TransactionsReadOnly.Any(f =>
                f.Action == TransactionActionType.SignUp))
            {
                return;
            }
            user.FinishRegistration();
            if (user.PhoneNumberConfirmed == false)
            {
                user.PhoneNumberConfirmed = true;
            }
            await _userRepository.UpdateAsync(user, token);
        }


    }
}
