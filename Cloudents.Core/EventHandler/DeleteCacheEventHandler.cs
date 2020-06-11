using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using static Cloudents.Core.EventHandler.CacheRegions;

namespace Cloudents.Core.EventHandler
{
    public class DeleteCacheEventHandler :
        IEventHandler<TransactionEvent>,
        IEventHandler<DocumentPriceChangeEvent>, 
        IEventHandler<DocumentFlaggedEvent>,
        IEventHandler<DocumentDeletedEvent>,
        IEventHandler<SubscribeToTutorEvent>,
        IEventHandler<DocumentCreatedEvent>,
        IEventHandler<TutorSubscriptionEvent>
    {
        private readonly ICacheProvider _cacheProvider;

        public DeleteCacheEventHandler(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public Task HandleAsync(DocumentPriceChangeEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion(DocumentById);
            _cacheProvider.DeleteRegion(ProfilePageDocument);
            return Task.CompletedTask;
        }
        public Task HandleAsync(TransactionEvent eventMessage, CancellationToken token)
        {
            //Document purchased
            if (eventMessage.Transaction is DocumentTransaction _)
            {
                
                _cacheProvider.DeleteRegion(DocumentById);
                _cacheProvider.DeleteRegion(ProfilePageDocument);
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(DocumentFlaggedEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion(DocumentById);
            _cacheProvider.DeleteRegion(ProfilePageDocument);
            return Task.CompletedTask;
        }

        public Task HandleAsync(DocumentDeletedEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion(DocumentById);
            _cacheProvider.DeleteRegion(ProfilePageDocument);
            return Task.CompletedTask;
        }
       

        public Task HandleAsync(SubscribeToTutorEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion(DocumentById);
            _cacheProvider.DeleteRegion(ProfilePageDocument);
            return Task.CompletedTask;
        }

        public Task HandleAsync(DocumentCreatedEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion(ProfilePageDocument);
            return Task.CompletedTask;
        }

        public Task HandleAsync(TutorSubscriptionEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteRegion(DocumentById);
            _cacheProvider.DeleteRegion(ProfilePageDocument);
            return Task.CompletedTask;
        }
    }

    public static class CacheRegions
    {
        public const string DocumentById = "document-by-id2";
        public const string ProfilePageDocument = "UserDocumentsQuery2";
    }
}