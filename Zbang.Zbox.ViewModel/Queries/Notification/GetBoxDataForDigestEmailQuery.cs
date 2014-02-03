using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Notification
{
    public class GetBoxDataForDigestEmailQuery : GetUserDigestQuery
    {
        public GetBoxDataForDigestEmailQuery(NotificationSettings notification, long boxid)
            : base(notification)
        {
            BoxId = boxid;
        }

        public long BoxId { get; private set; }
    }
}
