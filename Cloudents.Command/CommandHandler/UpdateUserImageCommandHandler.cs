using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateUserImageCommandHandler : ICommandHandler<UpdateUserImageCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public UpdateUserImageCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserImageCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.UpdateUserImage(message.ImagePath, message.FileName);
            await _userRepository.UpdateAsync(user, token);
        }
    }


    public class UpdateUserCoverImageCommandHandler : ICommandHandler<UpdateUserCoverImageCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public UpdateUserCoverImageCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserCoverImageCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.UpdateCoverImage(message.FileName);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}