using System;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetInviteDetailQuery
    {
        public GetInviteDetailQuery(Guid messageId)
        {
            MessageId = messageId;
        }
        public Guid MessageId { get; private set; }
    }
}
