using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class FinishRegistrationCommandHandler : ICommandHandler<FinishRegistrationCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public FinishRegistrationCommandHandler(IRepository<RegularUser> userRepository
            )
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(FinishRegistrationCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            user.FinishRegistration();
            await _userRepository.UpdateAsync(user, token);
        }


    }
}
