using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetCommentsLastUpdateQuery : BaseDigestLastUpdateQuery
    {
        public GetCommentsLastUpdateQuery(NotificationSetting notificationSettings, long boxid)
            : base(notificationSettings)
        {
            BoxId = boxid;
        }
        public long BoxId { get; private set; }
    }
}
