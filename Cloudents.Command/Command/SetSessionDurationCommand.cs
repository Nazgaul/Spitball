using System;
namespace Cloudents.Command.Command
{
    public class SetSessionDurationCommand : ICommand
    {
        public SetSessionDurationCommand(long tutorId, Guid sessionId, TimeSpan realDuration, double price)
        {
            TutorId = tutorId;
            SessionId = sessionId;
            RealDuration = realDuration;
            Price = price;
        }
        public long TutorId { get; }
        public Guid SessionId { get; }
        public TimeSpan RealDuration { get; }


        public double Price { get; }
    }
}
