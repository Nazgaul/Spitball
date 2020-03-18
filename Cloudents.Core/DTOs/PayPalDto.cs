namespace Cloudents.Core.DTOs
{
    public sealed class PayPalDto
    {
        public PayPalDto(string referenceId, decimal amount)
        {
            Amount = amount;
            ReferenceId = referenceId;
        }

        public decimal Amount { get; }
        public string ReferenceId { get; }
    }
}