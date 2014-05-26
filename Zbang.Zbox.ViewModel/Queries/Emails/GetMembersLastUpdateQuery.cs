using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetMembersLastUpdateQuery :BaseDigestLastUpdateQuery
    {
        public GetMembersLastUpdateQuery(NotificationSettings notificationSettings, long boxid)
            : base(notificationSettings)
        {
            BoxId = boxid;
        }
        public long BoxId { get; private set; }
    }
}
