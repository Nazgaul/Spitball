namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxQuery : QueryBase
    {

        public GetBoxQuery(long boxId, long userId)
            : base(userId)
        {
            BoxId = boxId;


        }
        public long BoxId { get; set; }
    }

}
