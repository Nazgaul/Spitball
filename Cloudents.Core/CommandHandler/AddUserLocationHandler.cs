using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc resolve")]
    public class AddUserLocationHandler : ICommandHandler<AddUserLocationCommand>
    {
        private readonly IRepository<UserLocation> _repository;

        public AddUserLocationHandler(IRepository<UserLocation> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(AddUserLocationCommand message, CancellationToken token)
        {
            var location = new UserLocation(message.User, message.Ip.ToString(), message.Country);

            await _repository.AddAsync(location, token);
        }
    }
}