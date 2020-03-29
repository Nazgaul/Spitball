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
        private readonly IRegularUserRepository _regularUserRepository;
        public FollowUserCommandHandler(IRepository<BaseUser> userRepository, IRegularUserRepository regularUserRepository)
        {
            _userRepository = userRepository;
            _regularUserRepository = regularUserRepository;
        }

        public async Task ExecuteAsync(FollowUserCommand message, CancellationToken token)
        {
            var followed = await _userRepository.LoadAsync(message.FollowedId, token);
            var follower = await _regularUserRepository.LoadAsync(message.FollowerId, token);
           
            followed.AddFollower(follower);
            await _userRepository.UpdateAsync(followed, token);
            
        }
    }
}
