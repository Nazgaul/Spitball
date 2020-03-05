namespace Cloudents.Core.DTOs
{
    public sealed class PayPalDto
    {
        public PayPalDto(string payPalId, decimal amount)
        {
            Amount = amount;
            PayPalId = payPalId;
        }

        public decimal Amount { get; }
        public string PayPalId { get; }
    }
}