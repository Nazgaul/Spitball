using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetUserByNotificationQuery : BaseDigestLastUpdateQuery, IPagedQuery
    {
        public GetUserByNotificationQuery(NotificationSetting notificationSettings,  int pageNumber , int rowsPerPage, int utcOffset)
            : base(notificationSettings)
        {
            NotificationSettings = notificationSettings;
            UtcOffset = utcOffset;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
        }

        public NotificationSetting NotificationSettings { get; }

        public int PageNumber { get; }
        public int RowsPerPage { get; }

        public int UtcOffset { get; private set; }

        public override string ToString()
        {
            return $"Notification {NotificationSettings.GetStringValue()} PageNumber {PageNumber} rows {RowsPerPage}";
        }
    }
}
