namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxSeoQuery : QueryBase
    {
        public GetBoxSeoQuery(long boxId, long userId)
            : base(userId)
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
    }
}
