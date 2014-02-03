using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetItemsLastUpdateQuery : BaseDigestLastUpdateQuery
    {
       public GetItemsLastUpdateQuery(NotificationSettings notificationSettings, long boxid)
           :base(notificationSettings)
       {
           BoxId = boxid;
       }
       public long BoxId { get; private set; }
      
    }
}
