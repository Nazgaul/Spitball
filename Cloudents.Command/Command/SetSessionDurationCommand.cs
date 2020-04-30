using System;
namespace Cloudents.Command.Command
{
    public class SetSessionDurationCommand : ICommand
    {
        public SetSessionDurationCommand(long tutorId, Guid sessionId, TimeSpan realDuration, long userId, decimal price)
        {
            TutorId = tutorId;
            SessionId = sessionId;
            RealDuration = realDuration;
            UserId = userId;
            Price = price;
        }
        public long TutorId { get; }
        public Guid SessionId { get; }
        public TimeSpan RealDuration { get; }

        public long UserId { get; }

        public decimal Price { get; }
    }
}
