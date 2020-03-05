using System;
namespace Cloudents.Command.Command
{
    public class SetSessionDurationCommand : ICommand
    {
        public SetSessionDurationCommand(long userId, Guid sessionId, TimeSpan realDuration)
        {
            UserId = userId;
            SessionId = sessionId;
            RealDuration = realDuration;
        }
        public long UserId { get; }
        public Guid SessionId { get; }
        public TimeSpan RealDuration { get; }
    }
}
