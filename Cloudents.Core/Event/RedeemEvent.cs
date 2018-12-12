using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class RedeemEvent : IEvent
    {
        public RedeemEvent(long userId, decimal amount)
        {
            UserId = userId;
            Amount = amount;
        }

        public long UserId { get; set; }
        public decimal Amount { get; set; }
    }
    
}
