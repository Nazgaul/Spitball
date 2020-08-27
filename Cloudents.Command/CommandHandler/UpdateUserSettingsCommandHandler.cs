using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateUserSettingsCommandHandler : ICommandHandler<UpdateUserSettingsCommand>
    {
        private readonly IRepository<User> _userRepository;

        public UpdateUserSettingsCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserSettingsCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.ChangeName(message.FirstName, message.LastName);
            if (user.Tutor != null)
            {
                user.Tutor.UpdateSettings(message.ShortParagraph,message.Title,message.Paragraph);
            }

            if (message.Avatar != null)
            {
                user.UpdateUserImage(message.Avatar);
            }
            if (message.Cover != null)
            {
                user.UpdateCoverImage(message.Cover);
            }
        }
    }
}
