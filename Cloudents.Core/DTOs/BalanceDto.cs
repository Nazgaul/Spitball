using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class BalanceDto
    {
        public BalanceDto(TransactionType transaction, decimal points)
        {
            Type = transaction.ToString("G");
            Points = points;
            Value = points/40;
        }

        public BalanceDto(string transaction, decimal points)
        {
            Type = transaction;
            Points = points;
            Value = points / 40;
        }

        public string Type { get; }
        public decimal Points { get; }

        public decimal Value { get;  }
    }
}