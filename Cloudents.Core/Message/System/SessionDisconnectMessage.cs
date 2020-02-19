using System;

namespace Cloudents.Core.Message.System
{
    public class SessionDisconnectMessage : ISystemQueueMessage
    {
        public SessionDisconnectMessage(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; private set; }
    }
}
