using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserCultureCommandHandler : ICommandHandler<UpdateUserCultureCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCultureCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserCultureCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.Culture = message.CultureInfo;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}