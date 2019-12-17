using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class FollowUserCommandHandler : ICommandHandler<FollowUserCommand>
    {
        private readonly IRepository<BaseUser> _userRepository;
        public FollowUserCommandHandler(IRepository<BaseUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(FollowUserCommand message, CancellationToken token)
        {
            var followed = await _userRepository.LoadAsync(message.FollowedId, token);
            var follower = await _userRepository.LoadAsync(message.FollowerId, token);

            if (!followed.Equals(follower))
            {
                followed.AddFollower(follower);
                await _userRepository.UpdateAsync(followed, token);
            }
        }
    }
}
