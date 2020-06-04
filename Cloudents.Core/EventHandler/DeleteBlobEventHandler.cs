using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class DeleteBlobEventHandler : IEventHandler<DocumentDeletedEvent>
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;

        public DeleteBlobEventHandler(IDocumentDirectoryBlobProvider blobProvider)
        {
            _blobProvider = blobProvider;
        }

        public Task HandleAsync(DocumentDeletedEvent eventMessage, CancellationToken token)
        {
            return _blobProvider.DeleteDirectoryAsync(eventMessage.Document.Id.ToString(), token);
        }
    }
}