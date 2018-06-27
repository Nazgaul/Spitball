using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        //private readonly IRepository<Transaction> _transactionRepository;


        

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
           // var rootTransaction = Transaction.UserCreateTransaction(message.User);
            //await _transactionRepository.AddAsync(rootTransaction,token);
            await _userRepository.AddAsync(message.User, token).ConfigureAwait(false);
        }
    }
}