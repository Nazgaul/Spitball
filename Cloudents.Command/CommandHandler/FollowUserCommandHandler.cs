using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class FollowUserCommandHandler : ICommandHandler<FollowUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        public FollowUserCommandHandler( IRegularUserRepository regularUserRepository)
        {
            _userRepository = regularUserRepository;
        }

        public async Task ExecuteAsync(FollowUserCommand message, CancellationToken token)
        {
            var followed = await _userRepository.LoadAsync(message.FollowedId, token);
            var follower = await _userRepository.LoadAsync(message.FollowerId, token);
            
            followed.AddFollower(follower);
            await _userRepository.UpdateAsync(followed, token);
        }
    }
}