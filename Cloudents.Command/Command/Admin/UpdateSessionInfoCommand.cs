using System;

namespace Cloudents.Command.Command.Admin
{
    public class UpdateSessionInfoCommand : ICommand
    {
        public Guid SessionId { get; set; }
        public int Minutes { get; set; }
        public UpdateSessionInfoCommand(Guid sessionId, int minutes)
        {
            SessionId = sessionId;
            Minutes = minutes;
        }
    }
}
