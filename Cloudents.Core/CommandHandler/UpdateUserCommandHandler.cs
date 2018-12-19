using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public UpdateUserCommandHandler(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserCommand message, CancellationToken token)
        {
            await _userRepository.UpdateAsync(message.User, token).ConfigureAwait(false);
        }
    }
}