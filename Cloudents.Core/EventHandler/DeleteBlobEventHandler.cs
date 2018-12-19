using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Storage;

namespace Cloudents.Application.EventHandler
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
            return _blobProvider.DeleteDirectoryAsync(eventMessage.Document.Id.ToString());
        }
    }
}