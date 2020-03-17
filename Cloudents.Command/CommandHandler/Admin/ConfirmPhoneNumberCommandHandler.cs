using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ConfirmPhoneNumberCommandHandler : ICommandHandler<ConfirmPhoneNumberCommand>
    {
        private readonly IRepository<User> _userRepository;

        public ConfirmPhoneNumberCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ConfirmPhoneNumberCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);

            user.ConfirmPhoneNumber();
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
