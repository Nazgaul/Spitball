using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class CashOutDto
    {
        public long UserId { get; set; }
        public decimal CashOutPrice { get; set; }
        public string UserEmail { get; set; }
        public DateTime CashOutTime { get; set; }

        public bool IsSuspect => FraudScore > 10 || UserQueryRatio < 0.2M;
        public int FraudScore { get; set; }
        public decimal UserQueryRatio { get; set; }
    }
}
