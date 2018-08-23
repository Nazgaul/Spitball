using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class CashOutDto
    {
        public long UserId { get; set; }
        public decimal CashOutPrice { get; set; }
        public DateTime CashOutTime { get; set; }

        //TODO:
        //return user email as well
        //add score from db 60
        //add the 3rd sql statement 0
        //isSuspect will be getter only that will compute from both properties

        public bool IsSuspect { get; set; }
    }
}
