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
        private readonly IRegularUserRepository _userRepository;

        public UpdateUserBalanceCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
        {
            await _userRepository.UpdateUsersBalance(token);
           
        }
    }
}