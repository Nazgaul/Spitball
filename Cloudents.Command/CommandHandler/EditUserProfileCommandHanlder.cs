using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class EditUserProfileCommandHanlder : ICommandHandler<EditUserProfileCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        public EditUserProfileCommandHanlder(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(EditUserProfileCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.ChangeName(message.Name, null);
            user.Description = message.Description;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
