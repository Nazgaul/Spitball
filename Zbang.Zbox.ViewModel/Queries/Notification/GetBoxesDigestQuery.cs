using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Notification
{
    public class GetBoxesDigestQuery : GetUserDigestQuery
    {
        public GetBoxesDigestQuery(NotificationSettings notification, long userId)
            : base( notification)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
