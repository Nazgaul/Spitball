using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class GiveTokensCommandHandler : ICommandHandler<GiveTokensCommand>
    {
       
        private readonly IUserRepository _userRepository;

        public GiveTokensCommandHandler(IUserRepository userRepository)
        {
            
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(GiveTokensCommand message, CancellationToken token)
        {
            TransactionType tType = message.TypeId;
             
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var p = Transaction.SendTokens(message.Price, tType);
            user.AddTransaction(p);
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}
