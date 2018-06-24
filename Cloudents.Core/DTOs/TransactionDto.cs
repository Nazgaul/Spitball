using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class TransactionDto
    {
        public DateTime Date { get; set; }
        public ActionType Action { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}