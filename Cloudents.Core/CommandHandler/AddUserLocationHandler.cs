using Cloudents.Core.Command;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler
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