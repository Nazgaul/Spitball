using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class DeleteBlobEventHandler : IEventHandler<DocumentDeletedEvent>
    {
        private readonly IBlobProvider<DocumentContainer> _blobProvider;

        public DeleteBlobEventHandler(IBlobProvider<DocumentContainer> blobProvider)
        {
            _blobProvider = blobProvider;
        }

        public Task HandleAsync(DocumentDeletedEvent eventMessage, CancellationToken token)
        {
            return _blobProvider.DeleteDirectoryAsync(eventMessage.Document.Id.ToString(), token);
        }
    }
}