using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceCommandHandler : ICommandHandler<UpdateUserBalanceCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEventStore _eventStore;

        public UpdateUserBalanceCommandHandler(IRegularUserRepository userRepository, ITransactionRepository transactionRepository, IEventStore eventStore)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _eventStore = eventStore;
        }


        public async Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
        {
            foreach (var id in message.UsersIds)
            {
                var user = await _userRepository.LoadAsync(id, token);
                user.Balance = await _transactionRepository.GetBalanceAsync(id, token);
                user.Score = (int)(await _transactionRepository.GetUserScoreAsync(id, token));
                
                await _userRepository.UpdateAsync(user, token);
                _eventStore.Add(new UpdateBalanceEvent(user));
            }
        }
    }
}