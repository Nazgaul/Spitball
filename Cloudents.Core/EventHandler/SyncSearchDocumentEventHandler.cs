using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class SyncSearchDocumentEventHandler : IEventHandler<DocumentCreatedEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public SyncSearchDocumentEventHandler(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(DocumentCreatedEvent eventMessage, CancellationToken token)
        {
            var doc = new Document()
            {
                University = eventMessage.Document.University.Id.ToString(),
                Country = eventMessage.Document.University.Country,
                Course = eventMessage.Document.Course.Name,
                DateTime = eventMessage.Document.TimeStamp.UpdateTime,
                Id = eventMessage.Document.Id.ToString(),
                Name = eventMessage.Document.Name,
                Tags = eventMessage.Document.Tags.Select(s => s.Name).ToArray(),
                Type = eventMessage.Document.Type
            };
            return _queueProvider.InsertMessageAsync(new DocumentSearchMessage(doc), token);
        }
    }
}