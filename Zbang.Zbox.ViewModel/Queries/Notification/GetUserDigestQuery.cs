using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Notification
{
    public class GetUserDigestQuery
    {
        public GetUserDigestQuery(NotificationSettings notification)
        {
            HourDiffForDigestEmail = (int)notification;
            Notification = notification;
        }
        public int HourDiffForDigestEmail { get; private set; }
        public NotificationSettings Notification { get; private set; }
    }
}
