using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class ExternalAuthDto
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Id { get; set; }

        public Language Language { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Email)}: {Email}, {nameof(Id)}: {Id}";
        }
    }

    public sealed class PayPalDto
    {
        public PayPalDto( string payPalId, decimal amount)
        {
            Amount = amount;
            PayPalId = payPalId;
        }

        public decimal Amount { get;  }
        public string PayPalId { get;  }
    }
}