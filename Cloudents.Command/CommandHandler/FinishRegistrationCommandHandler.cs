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
        private readonly IRepository<User> _userRepository;

        public FinishRegistrationCommandHandler(IRepository<User> userRepository
            )
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(FinishRegistrationCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            //if (user.Transactions.TransactionsReadOnly.Any(f =>
            //    f.Action == TransactionActionType.SignUp))
            //{
            //    return;
            //}
            user.FinishRegistration();
            await _userRepository.UpdateAsync(user, token);
        }


    }
}
