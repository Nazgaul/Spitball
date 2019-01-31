using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global",Justification = "Dto - json")]
    public class BalanceDto
    {
        public BalanceDto(TransactionType transaction, decimal points)
        {
            Name = transaction;
            Type = transaction.ToString("G");
            Points = points;
        }

        public string Type { get; }
        public TransactionType Name { get; }
        public decimal Points { get; }
    }
}