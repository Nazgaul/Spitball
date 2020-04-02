using System;
namespace Cloudents.Command.Command
{
    public class SetSessionDurationCommand : ICommand
    {
        public SetSessionDurationCommand(long tutorId, Guid sessionId, TimeSpan realDuration, long userId)
        {
            TutorId = tutorId;
            SessionId = sessionId;
            RealDuration = realDuration;
            UserId = userId;
        }
        public long TutorId { get; }
        public Guid SessionId { get; }
        public TimeSpan RealDuration { get; }

        public long UserId { get; }
    }
}
