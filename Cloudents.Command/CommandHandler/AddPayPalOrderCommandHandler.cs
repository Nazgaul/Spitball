using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddPayPalOrderCommandHandler : ICommandHandler<AddPayPalOrderCommand>
    {
        
        private readonly IRepository<UserToken> _userTokenRepository;
        private readonly IRepository<User> _userRepository;
        
        public AddPayPalOrderCommandHandler(IRepository<UserToken> userTokenRepository, IRepository<User> userRepository)
        {
            _userTokenRepository = userTokenRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(AddPayPalOrderCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            
            var userToken = new UserToken(user, message.Token);
            await _userTokenRepository.AddAsync(userToken, token);
        }
    }
}
