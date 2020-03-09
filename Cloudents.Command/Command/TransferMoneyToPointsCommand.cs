namespace Cloudents.Command.Command
{
    public class TransferMoneyToPointsCommand : ICommand
    {
        public TransferMoneyToPointsCommand(long userId, decimal amount, string payPalTransactionId, decimal localCurrencyPrice)
        {
            UserId = userId;
            Amount = amount;
            PayPalTransactionId = payPalTransactionId;
            LocalCurrencyPrice = localCurrencyPrice;
        }

        public long UserId { get; }

        public decimal Amount { get; }

        public string PayPalTransactionId { get; }
        public decimal LocalCurrencyPrice { get; }
    }
}