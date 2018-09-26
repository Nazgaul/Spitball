using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc resolve")]
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserCommand message, CancellationToken token)
        {
            await _userRepository.UpdateAsync(message.User, token).ConfigureAwait(false);
        }
    }


    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc resolve")]
    public class AddUserLoginHandler : ICommandHandler<AddUserLoginCommand>
    {
        private readonly IRepository<UserLogin> _repository;

        public AddUserLoginHandler( IRepository<UserLogin> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(AddUserLoginCommand message, CancellationToken token)
        {
            var login = new UserLogin(message.LoginProvider, message.ProviderKey, message.ProviderDisplayName,
                message.User);

            await _repository.AddAsync(login, token);
        }
    }
}