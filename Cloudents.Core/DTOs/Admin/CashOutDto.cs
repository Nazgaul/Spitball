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
        public decimal userQueryRatio { get; set; }
        //public static decimal AvgFraudScore { get; set; }


        public bool IsSuspect { get
            {
                return FraudScore > 20 || userQueryRatio < 0.2M;
            }
        }

    }
}
