using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
   public class UnFollowUserCommandHandler : ICommandHandler<UnFollowUserCommand>
    {
        private readonly IRepository<User> _userRepository;
        public UnFollowUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UnFollowUserCommand message, CancellationToken token)
        {
            var followed = await _userRepository.LoadAsync(message.TutorToFollow, token);
            var follower = await _userRepository.LoadAsync(message.UserId, token);
            followed.RemoveFollower(follower);
            await _userRepository.UpdateAsync(followed, token);
        }
    }
}
