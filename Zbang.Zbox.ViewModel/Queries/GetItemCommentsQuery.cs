
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetItemCommentsQuery : QueryBase
    {
        public GetItemCommentsQuery(long itemId, long userId)
            : base(userId)
        {
            ItemId = itemId;
        }

        public long ItemId { get; set; }

        
    }
}
