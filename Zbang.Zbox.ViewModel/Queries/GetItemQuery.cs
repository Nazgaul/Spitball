namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetItemQuery : QueryBase
    {
        public GetItemQuery(long userId, long itemId, long boxId)
            : base(userId)
        {
            ItemId = itemId;
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
        public long ItemId { get; }

    }
}
