using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetUserByNotificationQuery : BaseDigestLastUpdateQuery
    {
        public GetUserByNotificationQuery(NotificationSettings notificationSettings)
            :base(notificationSettings)
        {
            NotificationSettings = notificationSettings;
        }
        public NotificationSettings NotificationSettings { get; private set; }
    }
}
