using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Core.CommandHandler
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
            user.Culture = message.CultureInfo;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}