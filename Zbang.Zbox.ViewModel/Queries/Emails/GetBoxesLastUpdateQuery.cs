using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetBoxesLastUpdateQuery : BaseDigestLastUpdateQuery
    {
        public GetBoxesLastUpdateQuery(NotificationSettings notificationSettings, long userId)
            :base(notificationSettings)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
