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

        public async Task ExecuteAsync(IncrementDocumentNumberOfViewsCommand message, CancellationToken token)
        {
            await _documentRepository.UpdateNumberOfViewsAsync(message.Id, token);
        }
    }

    public class IncrementDocumentNumberOfDownloadsCommandHandler : ICommandHandler<IncrementDocumentNumberOfDownloadsCommand>
    {
        private readonly IDocumentRepository _documentRepository;

        public IncrementDocumentNumberOfDownloadsCommandHandler(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(IncrementDocumentNumberOfDownloadsCommand message, CancellationToken token)
        {
            await _documentRepository.UpdateNumberOfDownloadsAsync(message.Id, token);
        }
    }
}