using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc resolve")]
    public class AddUserLoginHandler : ICommandHandler<AddUserLoginCommand>
    {
        private readonly IRepository<UserLogin> _repository;

        public AddUserLoginHandler(IRepository<UserLogin> repository)
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