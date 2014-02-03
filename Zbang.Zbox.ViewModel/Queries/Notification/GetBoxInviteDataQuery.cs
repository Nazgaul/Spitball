namespace Zbang.Zbox.ViewModel.Queries.Notification
{
    public class GetBoxInviteDataQuery 
    {
        public GetBoxInviteDataQuery(long boxid)
        {
            BoxId = boxid;
        }
        public long BoxId { get; private set; }
    }
}
