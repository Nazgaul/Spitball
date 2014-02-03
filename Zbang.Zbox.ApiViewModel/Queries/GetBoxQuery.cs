

namespace Zbang.Zbox.ApiViewModel.Queries
{
    public class GetBoxQuery : QueryPagedBase
    {
        public GetBoxQuery(long boxId, long userId, int itemPage = 0)
            : base(userId, itemPage)
        {
            BoxId = boxId;


        }
        public long BoxId { get; set; }
    }
}
