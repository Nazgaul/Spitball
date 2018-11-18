using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
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
            await _documentRepository.UpdateNumberOfViews(message.Id, token);
        }
    }
}