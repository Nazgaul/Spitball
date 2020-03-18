using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class UndeleteBlobEventHandler : IEventHandler<DocumentUndeletedEvent>
    {
        private readonly IDocumentDirectoryBlobProvider _blobProvider;

        public UndeleteBlobEventHandler(IDocumentDirectoryBlobProvider blobProvider)
        {
            _blobProvider = blobProvider;
        }

        public Task HandleAsync(DocumentUndeletedEvent eventMessage, CancellationToken token)
        {
            return _blobProvider.UnDeleteDirectoryAsync(eventMessage.Document.Id.ToString(), token);
        }
    }
}
