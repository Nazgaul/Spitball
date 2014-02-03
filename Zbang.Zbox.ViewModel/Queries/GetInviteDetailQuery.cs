using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
