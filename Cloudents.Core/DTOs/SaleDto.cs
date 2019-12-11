using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs
{
    public class SaleDto
    {
        public long Id { get; set; }
        public string Course { get; set; }
        public string Info { get; set; }
        public SaleType Type { get; set; }
        public SalePaymentStatus Status { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string StudentName { get; set; }
        public long? Duration { get; set; }
        public string AnswerText { get; set; }
    }
}
