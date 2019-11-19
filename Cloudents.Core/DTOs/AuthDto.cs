using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class ExternalAuthDto
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Id { get; set; }

        public string Picture { get; set; }

        public Language Language { get; set; }

        public override string ToString()
        {
            return $"{nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Email)}: {Email}, {nameof(Id)}: {Id}, {nameof(Language)}: {Language}";
        }
    }

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