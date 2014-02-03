using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetCommentsLastUpdateQuery : BaseDigestLastUpdateQuery
    {
        public GetCommentsLastUpdateQuery(NotificationSettings notificationSettings, long boxid)
            : base(notificationSettings)
        {
            BoxId = boxid;
        }
        public long BoxId { get; private set; }


    }
}
