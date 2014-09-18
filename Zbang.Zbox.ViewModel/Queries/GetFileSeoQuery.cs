

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetFileSeoQuery
    {
        public GetFileSeoQuery(long itemId)
        {
            ItemId = itemId;
        }

        public long ItemId { get; private set; }
    }
}
