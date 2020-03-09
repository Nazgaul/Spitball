namespace Cloudents.Core.DTOs
{
    public sealed class PayPalDto
    {
        public PayPalDto(string payPalId, decimal amount, decimal localCurrencyAmount)
        {
            Amount = amount;
            PayPalId = payPalId;
            LocalCurrencyAmount = localCurrencyAmount;
        }

        public decimal Amount { get; }
        public string PayPalId { get; }
        public decimal LocalCurrencyAmount { get; }
    }
}