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

        public UpdateUserBalanceCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
        {
            foreach (var id in message.UsersIds)
            {
                var user = await _userRepository.LoadAsync(id, token);

                user.UpdateUserBalance();
                await _userRepository.UpdateAsync(user, token);

            }
        }
    }
}