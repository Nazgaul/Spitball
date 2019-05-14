//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command.Command;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;
//using JetBrains.Annotations;

//namespace Cloudents.Command.CommandHandler
//{
//    [UsedImplicitly]
//    public class CreateUrlStatsCommandHandler : ICommandHandler<CreateUrlStatsCommand>
//    {
//        private readonly IRepository<UrlStats> _repository;

//        public CreateUrlStatsCommandHandler(IRepository<UrlStats> repository)
//        {
//            _repository = repository;
//        }

//        public Task ExecuteAsync(CreateUrlStatsCommand message, CancellationToken token)
//        {
//            var urlStats = new UrlStats(message.Host,
//                message.DateTime,
//                message.UrlTarget,
//                message.UrlSource,
//                message.SourceLocation,
//                message.Ip);

//            return _repository.AddAsync(urlStats, token);
//        }
//    }
//}