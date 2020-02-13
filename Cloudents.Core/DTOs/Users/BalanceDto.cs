using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs.Users
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Dto - json")]
    public class BalanceDto
    {
        public BalanceDto(string transaction, decimal points, decimal value, string symbol)
        {
            Type = transaction;
            Points = points;
            Value = value;
            Symbol = symbol;
        }

        public string Type { get; }
        public decimal Points { get; }

        public decimal Value { get; }

        public string Symbol { get; set; }
    }
}