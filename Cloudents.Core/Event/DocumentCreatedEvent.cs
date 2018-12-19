using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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