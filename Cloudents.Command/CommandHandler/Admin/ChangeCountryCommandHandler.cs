using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
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
            var user = await _userRepository.LoadAsync(message.Id, token);
            user.ChangeCountryAdmin(message.Country.ToUpperInvariant());
            await _userRepository.UpdateAsync(user, token);

        }
    }
}
