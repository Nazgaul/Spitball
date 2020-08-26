using System;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.Message.Email
{
    public class RequestTutorMessage : ISystemQueueMessage
    {
        public RequestTutorMessage(Guid leadId)
        {
            LeadId = leadId;
        }

        public Guid LeadId{ get; private set; }
    }
}