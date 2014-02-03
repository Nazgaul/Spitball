
using System.Runtime.Serialization;


namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetItemCommentsQuery : QueryBase
    {
        public GetItemCommentsQuery(long itemId, long userId)
            : base(userId)
        {
            this.ItemId = itemId;
        }

        public long ItemId { get; set; }

        
    }
}
