using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class CashOutDto
    {
        public long UserId { get; set; }
        public decimal CashOutPrice { get; set; }
        public string UserEmail { get; set; }
        public DateTime CashOutTime { get; set; }
        public int? FraudScore { get; set; }
        public decimal UserQueryRatio { get; set; }
        //public static decimal AvgFraudScore { get; set; }
        public bool? Approved { get; set; }
        public string DeclinedReason { get; set; }
        public bool IsIsrael { get; set; }
        public bool IsSuspect => FraudScore > 5000 || UserQueryRatio < 0.2M;
        public Guid TransactionId { get; set; }
    }
}
