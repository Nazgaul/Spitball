using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DistributeTokensCommandHandler : ICommandHandler<DistributeTokensCommand>
    {

        private readonly IUserRepository _userRepository;

        public DistributeTokensCommandHandler(IUserRepository userRepository)
        {

            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(DistributeTokensCommand message, CancellationToken token)
        {

            var user = await _userRepository.GetAsync(message.UserId, token);
            if (user == null)
            {
                //User not exists not crashing the system.
                return;
            }
            var transaction = Transaction.DistributeTokens(message.ActionType,message.TransactionType, message.Price);
            user.AddTransaction(transaction);
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}
