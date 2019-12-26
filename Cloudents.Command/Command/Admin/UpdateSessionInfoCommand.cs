using System;

namespace Cloudents.Command.Command.Admin
{
    public class UpdateSessionInfoCommand : ICommand
    {
        public Guid SessionId { get; set; }
        public int DurationInMinutes { get; set; }
        public UpdateSessionInfoCommand(Guid sessionId, int durationInMinutes)
        {
            SessionId = sessionId;
            DurationInMinutes = durationInMinutes;
        }
    }
}
