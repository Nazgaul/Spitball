using System.Diagnostics.CodeAnalysis;
using Cloudents.Common.Enum;

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
            Value = points / 40;
        }

        //public BalanceDto(string transaction, decimal points)
        //{
        //    Type = transaction;
        //    Points = points;
        //    Value = points / 40;
        //}

        public string Type { get; }
        public TransactionType Name { get; }
        public decimal Points { get; }

        public decimal Value { get; }
    }
}