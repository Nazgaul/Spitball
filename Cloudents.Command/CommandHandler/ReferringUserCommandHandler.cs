using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class ReferringUserCommandHandler : ICommandHandler<ReferringUserCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IEventStore _eventStore;

        public ReferringUserCommandHandler(IRepository<RegularUser> userRepository, IRepository<Transaction> transactionRepository, IEventStore eventStore)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _eventStore = eventStore;
        }

        public async Task ExecuteAsync(ReferringUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.InvitingUserId, token);
            var register = await _userRepository.LoadAsync(message.RegisteredUserId, token);
            var tx = new Transaction(TransactionActionType.ReferringUser, TransactionType.Earned, ReputationAction.ReferringUser, user)
            {
                InvitedUser = register

            };
            _eventStore.Add(new TransactionEvent(tx));
            await _transactionRepository.AddAsync(tx, default);
        }
    }
}