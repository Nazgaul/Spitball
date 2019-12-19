using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class DocumentUndeletedEvent : IEvent
    {
        public DocumentUndeletedEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }

    public class DocumentPriceChangeEvent : IEvent
    {
        public DocumentPriceChangeEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }


    public class DocumentFlaggedEvent : IEvent
    {
        public DocumentFlaggedEvent(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }
}
