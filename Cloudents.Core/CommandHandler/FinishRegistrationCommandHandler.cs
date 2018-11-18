using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.CommandHandler
{
    public class FinishRegistrationCommandHandler : ICommandHandler<FinishRegistrationCommand>
    {
        private readonly IUserRepository _userRepository;

        public FinishRegistrationCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(FinishRegistrationCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            if (!user.Transactions.Any(a => a.Action == ActionType.SignUp))
            {
                var t = Transaction.UserCreate();
                user.AddTransaction(t);
            }
        }


    }
}
