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

        public AddUserLocationCommandHandler(IRepository<UserLocation> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(AddUserLocationCommand message, CancellationToken token)
        {
            var location = new UserLocation(message.User, message.Ip.ToString(), message.Country, message.FingerPrint, message.UserAgent);

            await _repository.AddAsync(location, token);
        }
    }
}