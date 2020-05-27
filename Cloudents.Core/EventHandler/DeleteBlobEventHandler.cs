using System.Collections.Generic;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class DeleteBlobEventHandler : IEventHandler<DocumentDeletedEvent>, IEventHandler<DeleteUserEvent>
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

        public async Task HandleAsync(DeleteUserEvent eventMessage, CancellationToken token)
        {
            var tasks = new List<Task>();
            foreach (var document in eventMessage.User.Documents)
            {
                tasks.Add(  _blobProvider.DeleteDirectoryAsync(document.Id.ToString(), token));
            }

            await Task.WhenAll(tasks);
        }
    }
}