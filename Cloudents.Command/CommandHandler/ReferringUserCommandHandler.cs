using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ReferringUserCommandHandler : ICommandHandler<ReferringUserCommand>
    {
        private readonly IRepository<User> _userRepository;

        public ReferringUserCommandHandler(IRepository<User> userRepository
           )
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ReferringUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.InvitingUserId, token);
            var v = user.Transactions.TransactionsReadOnly.Count(c => c is ReferUserTransaction);
            if (v >= 5)
            {
                return;
            }
            var register = await _userRepository.LoadAsync(message.RegisteredUserId, token);
            user.ReferUser(register);
            await _userRepository.UpdateAsync(user, token);
            //var tx = new Transaction(TransactionActionType.ReferringUser, TransactionType.Earned, ReputationAction.ReferringUser, user)
            //{
            //    InvitedUser = register

            //};
            ////tx.Events.Add(new TransactionEvent(tx));
            //await _transactionRepository.AddAsync(tx, default);
        }
    }
}