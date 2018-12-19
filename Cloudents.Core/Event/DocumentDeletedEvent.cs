using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
{
    public class DocumentDeletedEvent : IEvent
    {
        public DocumentDeletedEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }
}