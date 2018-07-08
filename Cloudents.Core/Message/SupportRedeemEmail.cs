using System;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class SupportRedeemEmail: BaseEmail
    {
        public SupportRedeemEmail(decimal amount, long userId) : base("support@spitball.co", null, "Redeem Email")
        {
            Amount = amount;
            UserId = userId;
        }

        public decimal Amount { get; set; }
        public long UserId { get; set; }
       

        public override string ToString()
        {
            return $"User id: {UserId} want to redeem {Amount}";
        }
    }
}