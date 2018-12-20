using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.CommandHandler.Admin
{
    public class ChangeCountryCommandHandler : ICommandHandler<ChangeCountryCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public ChangeCountryCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ChangeCountryCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            user.Country = message.Country.ToUpperInvariant();
            await _userRepository.UpdateAsync(user, token);

        }
    }
}
