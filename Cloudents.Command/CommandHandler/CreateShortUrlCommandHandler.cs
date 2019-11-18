using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateShortUrlCommandHandler : ICommandHandler<CreateShortUrlCommand>
    {
        private readonly IRepository<ShortUrl> _repository;

        public CreateShortUrlCommandHandler(IRepository<ShortUrl> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(CreateShortUrlCommand message, CancellationToken token)
        {
            var shortUrl = new ShortUrl(message.Identifier, message.Destination, message.Expiration);
            await _repository.AddAsync(shortUrl, token);
        }
    }
}