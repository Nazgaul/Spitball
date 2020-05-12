namespace Cloudents.Command.Command
{
    public class TransferMoneyToPointsCommand : ICommand
    {
        public TransferMoneyToPointsCommand(long userId, decimal amount, string transactionId)
        {
            UserId = userId;
            Amount = amount;
            TransactionId = transactionId;
        }

        public long UserId { get; }

        public decimal Amount { get; }

        public string TransactionId { get; }
    }
}