using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceCommandHandler : ICommandHandler<UpdateUserBalanceCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;

        public UpdateUserBalanceCommandHandler(IRegularUserRepository userRepository,
            ITransactionRepository transactionRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }


        public async Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
        {
            foreach (var id in message.UsersIds)
            {
                //TODO: we can do it better
                var user = await _userRepository.LoadAsync(id, token);
                var balance = await _transactionRepository.GetBalanceAsync(id, token);
                var score = (int)(await _transactionRepository.GetUserScoreAsync(id, token));
                user.UpdateUserBalance(balance, score);
                await _userRepository.UpdateAsync(user, token);

            }
        }
    }
}