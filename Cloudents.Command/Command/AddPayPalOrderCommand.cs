
using System;

namespace Cloudents.Command.Command
{
    public class AddPayPalOrderCommand : ICommand
    {
        public AddPayPalOrderCommand(long userId, string payPalOrderId, Guid sessionId)
        {
            PayPalOrderId = payPalOrderId;
            SessionId = sessionId;
            UserId = userId;
        }
        public string PayPalOrderId { get; }
        public long UserId { get; }
        public Guid SessionId { get; }
    }
}
