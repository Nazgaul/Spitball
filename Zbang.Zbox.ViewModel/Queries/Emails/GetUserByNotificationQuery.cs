using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetUserByNotificationQuery : BaseDigestLastUpdateQuery, IPagedQuery
    {
        public GetUserByNotificationQuery(NotificationSettings notificationSettings, int pageNumber = 0, int rowsPerPage = int.MaxValue)
            : base(notificationSettings)
        {
            NotificationSettings = notificationSettings;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
        }

        public NotificationSettings NotificationSettings { get; }

        public int PageNumber { get; }
        public int RowsPerPage { get; }
    }
}
