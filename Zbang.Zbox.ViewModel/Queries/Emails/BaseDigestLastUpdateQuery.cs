using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public abstract class BaseDigestLastUpdateQuery
    {
        public const int OnEveryChangeTimeToQueryInMinutes = 15;
        protected BaseDigestLastUpdateQuery(NotificationSetting notificationSettings)
        {
            NotificationSettings = notificationSettings;
        }

        private NotificationSetting NotificationSettings { get; }

        public int MinutesPerNotificationSettings
        {
            get
            {
                //if (NotificationSettings == NotificationSettings.OnEveryChange)
                //{
                //    return (int)TimeSpan.FromMinutes(OnEveryChangeTimeToQueryInMinutes).TotalMinutes;
                //}
                return (int)NotificationSettings * (int)TimeSpan.FromHours(1).TotalMinutes;
            }
        }
    }
}
