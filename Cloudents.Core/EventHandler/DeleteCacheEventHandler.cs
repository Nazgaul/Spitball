using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public class DeleteCacheEventHandler : IEventHandler<TransactionEvent>, IEventHandler<DocumentPriceChangeEvent>, IEventHandler<DocumentFlaggedEvent>
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
                _cacheProvider.DeleteRegion("document-by-id");
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(DocumentPriceChangeEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion("document-by-id");
            return Task.CompletedTask;
        }

        public Task HandleAsync(DocumentFlaggedEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion("document-by-id");
            return Task.CompletedTask;
        }
    }
}