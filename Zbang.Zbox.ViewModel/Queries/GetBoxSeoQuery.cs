namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxSeoQuery //: QueryBase
    {
        public GetBoxSeoQuery(long boxId)
            //: base(userId)
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
    }
}
