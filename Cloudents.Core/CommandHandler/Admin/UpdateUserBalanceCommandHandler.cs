using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceCommandHandler : ICommandHandler<UpdateUserBalanceCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserBalanceCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
        {
            foreach (var userId in message.UserIds)
            {
                var balance = await _userRepository.UserBalanceAsync(userId, token);
                var user = await _userRepository.LoadAsync(userId, token);
                user.Balance = balance;
                await _userRepository.UpdateAsync(user, token);
            }
        }
    }
}