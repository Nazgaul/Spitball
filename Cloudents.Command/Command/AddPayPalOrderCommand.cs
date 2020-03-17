
using System;

namespace Cloudents.Command.Command
{
    public class AddPayPalOrderCommand : ICommand
    {
        public AddPayPalOrderCommand(long userId, string token, Guid sessionId)
        {
            Token = token;
            SessionId = sessionId;
            UserId = userId;
        }
        public string Token { get; }
        public long UserId { get; }
        public Guid SessionId { get; }
    }
}
