using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetBoxLastUpdateQuery: BaseDigestLastUpdateQuery
    {
        public GetBoxLastUpdateQuery(NotificationSettings notificationSettings, long boxid)
           :base(notificationSettings)
       {
           BoxId = boxid;
       }
       public long BoxId { get; private set; }
      
    }
}
