namespace Cloudents.Command.Documents.ChangePrice
{
    public class ChangeDocumentPriceCommand : ICommand
    {
        public ChangeDocumentPriceCommand(long documentId, long userId, decimal price)
        {
            DocumentId = documentId;
            UserId = userId;
            Price = price;
        }

        public long DocumentId { get; }
        public long UserId { get; }
        public decimal Price { get; }

    }
}
