using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class DocumentCreatedEvent : IEvent
    {
        public DocumentCreatedEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }
}