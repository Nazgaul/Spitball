using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddPayPalOrderCommandHandler : ICommandHandler<AddPayPalOrderCommand>
    {
        private readonly IRepository<User> _userRepository;
        
        public AddPayPalOrderCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(AddPayPalOrderCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.AddToken(message.Token);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
