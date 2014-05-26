namespace Zbang.Zbox.ViewModel.Queries.Emails
{
    public class GetBadItemFlagQuery : QueryBase
    {
        public GetBadItemFlagQuery(long userId, long itemId) : base(userId)
        {
            ItemId = itemId;
        }

        public long ItemId { get; private set; }
    }
}
