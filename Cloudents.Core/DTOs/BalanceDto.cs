using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class BalanceDto
    {
        public BalanceDto(TransactionType transaction, decimal points)
        {
            Transaction = transaction;
            Points = points;
            Value = points/40;
        }

        public TransactionType Transaction { get; }
        public decimal Points { get; }

        public decimal Value { get;  }
    }
}