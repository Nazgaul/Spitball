namespace Zbang.Zbox.ViewModel.Queries.Notification
{
    public class GetBoxDataForImmediateEmailQuery
    {
        public GetBoxDataForImmediateEmailQuery(long boxid,long itemId)
        {
            BoxId = boxid;
            ItemId = itemId;
        }
        public long BoxId { get; private set; }
        public long ItemId { get; private set; }
    }
}
