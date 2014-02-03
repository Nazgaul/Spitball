using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Notification
{
    public class GetBoxSubscribers
    {
        public GetBoxSubscribers(long boxid, NotificationSettings notificationSetttings)
        {
            BoxId = boxid;
            NotificationSettings = notificationSetttings;
        }

        public long BoxId { get; private set; }
        public NotificationSettings NotificationSettings { get; set; }
    }
}
