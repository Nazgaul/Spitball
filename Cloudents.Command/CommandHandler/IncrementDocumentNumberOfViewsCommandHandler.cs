using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class IncrementDocumentNumberOfViewsCommandHandler : ICommandHandler<IncrementDocumentNumberOfViewsCommand>
    {
        private readonly IDocumentRepository _documentRepository;

        public IncrementDocumentNumberOfViewsCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public Task ExecuteAsync(IncrementDocumentNumberOfViewsCommand message, CancellationToken token)
        {
            return _documentRepository.UpdateNumberOfViewsAsync(message.Id, token);
        }
    }

    public class IncrementDocumentNumberOfDownloadsCommandHandler : ICommandHandler<IncrementDocumentNumberOfDownloadsCommand>
    {
        private readonly IDocumentRepository _documentRepository;

        public IncrementDocumentNumberOfDownloadsCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public Task ExecuteAsync(IncrementDocumentNumberOfDownloadsCommand message, CancellationToken token)
        {
            return _documentRepository.UpdateNumberOfDownloadsAsync(message.Id, token);
        }
    }
}