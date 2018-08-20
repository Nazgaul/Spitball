using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    class CreateSendTokensCommandHandler : ICommandHandler<CreateSendTokensCommand>
    {
       
        private readonly IUserRepository _userRepository;

        public CreateSendTokensCommandHandler(IUserRepository userRepository)
        {
            
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(CreateSendTokensCommand message, CancellationToken token)
        {
            Int64 UserId = System.Convert.ToInt64(message.UserId);
            var user = await _userRepository.LoadAsync(UserId, token);
            var p = Transaction.SendTokens(message.Price, Enum.TransactionType.Awarded);
            user.AddTransaction(p);
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}
