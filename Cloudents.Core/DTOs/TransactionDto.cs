using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class TransactionDto
    {
        public DateTime Date { get; set; }
        public string Action { get; set; } //TODO: this was an enum
        public string Type { get; set; } //TODO: this was an enum
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}