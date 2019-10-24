using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global",Justification = "Dto - json")]
    public class BalanceDto
    {
        public BalanceDto(string transaction, decimal points, string value)
        {
            //Name = transaction;
            Type = transaction;
            Points = points;
            Value = value;
        }

        public string Type { get; }
        //public TransactionType Name { get; }
        public decimal Points { get; }

        public string Value { get; }
    }
}