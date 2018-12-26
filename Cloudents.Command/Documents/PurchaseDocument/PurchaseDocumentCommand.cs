namespace Cloudents.Command.Documents.PurchaseDocument
{
    public class PurchaseDocumentCommand : ICommand
    {
        public PurchaseDocumentCommand(long documentId, long userId)
        {
            DocumentId = documentId;
            UserId = userId;
        }

        public long DocumentId { get; private set; }
        public long UserId { get; private set; }
    }
}
