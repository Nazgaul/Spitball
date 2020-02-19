using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public class DeleteCacheEventHandler : IEventHandler<TransactionEvent>,
        IEventHandler<DocumentPriceChangeEvent>, 
        IEventHandler<DocumentFlaggedEvent>,
        IEventHandler<DocumentDeletedEvent>
    {
        private readonly ICacheProvider _cacheProvider;

        public DeleteCacheEventHandler(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            if (eventMessage.Transaction is DocumentTransaction _)
            {
                return RemoveDocumentFromCacheAsync();
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(DocumentPriceChangeEvent eventMessage, CancellationToken token)
        {
            return RemoveDocumentFromCacheAsync();
        }

        public Task HandleAsync(DocumentFlaggedEvent eventMessage, CancellationToken token)
        {
            return RemoveDocumentFromCacheAsync();
        }

        public Task HandleAsync(DocumentDeletedEvent eventMessage, CancellationToken token)
        {
            return RemoveDocumentFromCacheAsync();
        }

        private Task RemoveDocumentFromCacheAsync()
        {
            _cacheProvider.DeleteRegion("document-by-id");
            return Task.CompletedTask;
        }
    }
}