namespace Cloudents.Command.Command
{
    public class TransferMoneyToPointsCommand : ICommand
    {
        public TransferMoneyToPointsCommand(long userId, decimal amount, string payPalTransactionId)
        {
            UserId = userId;
            Amount = amount;
            PayPalTransactionId = payPalTransactionId;
        }

        public long UserId { get; }

        public decimal Amount { get; }

        public string PayPalTransactionId { get; }
    }
}