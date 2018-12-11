using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceCommandHandler : ICommandHandler<UpdateUserBalanceCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;

        public UpdateUserBalanceCommandHandler(IUserRepository UserRepository, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = UserRepository;
        }

        public async Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
        {
            foreach (var Id in message.UsersIds)
            {
                var user = await _userRepository.LoadAsync(Id, token);
                user.Balance = await _transactionRepository.GetBalanceAsync(Id, token);
                await _userRepository.UpdateAsync(user, token);
            }
        }
    }
}