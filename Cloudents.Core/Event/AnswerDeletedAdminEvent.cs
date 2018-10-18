using Cloudents.Core.Interfaces;
using System.Collections.Generic;


namespace Cloudents.Core.Event
{
    public class AnswerDeletedAdminEvent : IEvent
    {
        public AnswerDeletedAdminEvent(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        public IEnumerable<long> UserIds { get; }
    }
}
