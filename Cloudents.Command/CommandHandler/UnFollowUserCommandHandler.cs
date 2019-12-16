using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UnFollowUserCommandHandler : ICommandHandler<UnFollowUserCommand>
    {
        private readonly IRepository<BaseUser> _userRepository;
        public UnFollowUserCommandHandler(IRepository<BaseUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UnFollowUserCommand message, CancellationToken token)
        {
            var followed = await _userRepository.LoadAsync(message.FollowedId, token);
            var follower = await _userRepository.LoadAsync(message.FollowerId, token);


            followed.RemoveFollower(follower);
            await _userRepository.UpdateAsync(followed, token);
        }
    }
}
