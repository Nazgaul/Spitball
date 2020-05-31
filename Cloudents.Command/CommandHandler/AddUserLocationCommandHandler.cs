using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc resolve")]
    public class AddUserLocationCommandHandler : ICommandHandler<AddUserLocationCommand>
    {
        private readonly IRepository<UserLocation> _repository;
        private readonly IIpToLocation _ipToLocation;

        public AddUserLocationCommandHandler(IRepository<UserLocation> repository, IIpToLocation ipToLocation)
        {
            _repository = repository;
            _ipToLocation = ipToLocation;
        }

        public async Task ExecuteAsync(AddUserLocationCommand message, CancellationToken token)
        {
            var result = await _ipToLocation.GetLocationAsync(message.Ip.ToString(), token);
            var location = new UserLocation(message.User, message.Ip.ToString(), result!.CountryCode,  message.UserAgent);

            await _repository.AddAsync(location, token);
        }
    }
}