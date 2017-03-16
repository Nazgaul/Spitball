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

        public int MinutesPerNotificationSettings => (int)NotificationSettings * (int)TimeSpan.FromHours(1).TotalMinutes;
    }
}
