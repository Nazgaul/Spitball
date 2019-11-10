using Cloudents.Core.DTOs.SearchSync;

namespace Cloudents.Core.Message.System
{
    public class DocumentSearchMessage : ISystemQueueMessage
    {
        public bool ShouldInsert { get; private set; }
        public DocumentSearchMessage(DocumentSearchDto document, bool shouldInsert)
        {
            Document = document;
            ShouldInsert = shouldInsert;
        }

        public DocumentSearchDto Document { get; private set; }
    }
}