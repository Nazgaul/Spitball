using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
            TransactionType tType = (TransactionType)message.TypeId;
             
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var p = Transaction.SendTokens(message.Price, tType);
            user.AddTransaction(p);
            await _userRepository.UpdateAsync(user, token).ConfigureAwait(false);
        }
    }
}
