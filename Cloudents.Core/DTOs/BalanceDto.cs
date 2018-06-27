using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class BalanceDto
    {
        public BalanceDto(TransactionType transaction, decimal balance)
        {
            Transaction = transaction;
            Balance = balance;
        }

        public TransactionType Transaction { get; }
        public decimal Balance { get; }
    }
}