using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlockChainErc20Service _blockChainErc20Service;


        private const decimal InitialBalance = 100;

        public CreateUserCommandHandler(IUserRepository userRepository,  IBlockChainErc20Service blockChainErc20Service)
        {
            _userRepository = userRepository;
            _blockChainErc20Service = blockChainErc20Service;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            var (privateKey, _) = _blockChainErc20Service.CreateAccount();
            message.User.AddTransaction(ActionType.SignUp, TransactionType.Awarded, InitialBalance);
            message.User.PrivateKey = privateKey;
            await _userRepository.AddAsync(message.User, token).ConfigureAwait(false);
        }
    }
}