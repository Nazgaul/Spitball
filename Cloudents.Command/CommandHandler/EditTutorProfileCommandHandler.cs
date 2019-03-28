using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class EditTutorProfileCommandHandler: ICommandHandler<EditTutorProfileCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public EditTutorProfileCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(EditTutorProfileCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.ChangeName(message.Name, message.LastName);
            user.Description = message.Description;
            user.Tutor.Bio = message.Bio;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
