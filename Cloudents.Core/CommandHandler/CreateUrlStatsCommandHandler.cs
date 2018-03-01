using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateUrlStatsCommandHandler : ICommandHandlerAsync<CreateUrlStatsCommand>
    {
        private readonly IRepository<UrlStats> _repository;

        public CreateUrlStatsCommandHandler(IRepository<UrlStats> repository)
        {
            _repository = repository;
        }

        public Task HandleAsync(CreateUrlStatsCommand message, CancellationToken token)
        {
            var urlStats = new UrlStats(message.Host,
                message.DateTime,
                message.UrlTarget,
                message.UrlSource,
                message.SourceLocation,
                message.Ip);

            return _repository.SaveAsync(urlStats, token);
        }
    }
}