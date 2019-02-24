using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ConfirmePhoneNumberCommandHandler : ICommandHandler<ConfirmPhoneNumberCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public ConfirmePhoneNumberCommandHandler(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ConfirmPhoneNumberCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
         
            user.ConfirmePhoneNumber();
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
