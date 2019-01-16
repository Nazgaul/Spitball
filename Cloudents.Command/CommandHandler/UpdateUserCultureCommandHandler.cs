using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserCultureCommandHandler : ICommandHandler<UpdateUserCultureCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public UpdateUserCultureCommandHandler(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserCultureCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.Language = message.CultureInfo;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}