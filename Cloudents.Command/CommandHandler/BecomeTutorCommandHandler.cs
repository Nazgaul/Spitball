using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class BecomeTutorCommandHandler : ICommandHandler<BecomeTutorCommand>
    {

        private readonly IRegularUserRepository _userRepository;

        public BecomeTutorCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(BecomeTutorCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            user.Tutor = new Tutor(message.Bio, message.Price, user.Id);

            await _userRepository.UpdateAsync(user, token);
        }
    }
}