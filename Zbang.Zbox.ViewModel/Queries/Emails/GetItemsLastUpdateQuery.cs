﻿using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetItemsLastUpdateQuery : BaseDigestLastUpdateQuery
    {
       public GetItemsLastUpdateQuery(NotificationSetting notificationSettings, long boxid)
           :base(notificationSettings)
       {
           BoxId = boxid;
       }
       public long BoxId { get; private set; }
      
    }
}
