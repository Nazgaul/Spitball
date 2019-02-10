namespace Cloudents.Command.Documents.ChangePrice
{
    public class ChangePriceCommand : ICommand
    {
        public ChangePriceCommand(long documentId, long userId, decimal price)
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
