using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
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