using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class CashOutDto
    {
        public long UserId { get; set; }
        public decimal CashOutPrice { get; set; }
        public string UserEmail { get; set; }
        public DateTime CashOutTime { get; set; }

        public bool? Approved { get; set; }
        public string DeclinedReason { get; set; }
        public string Country { get; set; }
        public Guid TransactionId { get; set; }

        public int ReferCount { get; set; }
        public int SoldDocument { get; set; }
        public int CorrectAnswer { get; set; }
        public int SoldDeletedDocument { get; set; }
        public int DeletedCorrectAnswer { get; set; }
        public int CashOut { get; set; }
        public int AwardCount { get; set; }
        public int BuyCount { get; set; }
    }
}
