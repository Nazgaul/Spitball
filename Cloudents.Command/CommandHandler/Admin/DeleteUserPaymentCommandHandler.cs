using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteUserPaymentCommandHandler : ICommandHandler<DeleteUserPaymentCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        public DeleteUserPaymentCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(DeleteUserPaymentCommand command, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(command.UserId, token);
            user.DeleteUserPayment();
        }
    }
}
