using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public abstract class BaseDigestLastUpdateQuery
    {
        public const int OnEveryChangeTimeToQueryInMInutes = 15;
        protected BaseDigestLastUpdateQuery(NotificationSettings notificationSettings)
        {
            NotificationSettings = notificationSettings;
        }

        private NotificationSettings NotificationSettings { get; set; }

        public int MinutesPerNotificationSettings
        {
            get
            {
                if (NotificationSettings == Infrastructure.Enums.NotificationSettings.OnEveryChange)
                {
                    return (int)TimeSpan.FromMinutes(OnEveryChangeTimeToQueryInMInutes).TotalMinutes;
                }
                return (int)NotificationSettings * (int)TimeSpan.FromHours(1).TotalMinutes;
            }
        }
    }
}
