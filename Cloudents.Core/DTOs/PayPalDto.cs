namespace Cloudents.Core.DTOs
{
    public sealed class PayPalDto
    {
        public PayPalDto(string referenceId, decimal amount, decimal localCurrencyAmount)
        {
            Amount = amount;
            ReferenceId = referenceId;
            LocalCurrencyAmount = localCurrencyAmount;
        }

        public decimal Amount { get; }
        public string ReferenceId { get; }
        public decimal LocalCurrencyAmount { get; }
    }
}