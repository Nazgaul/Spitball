namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxIdQuery 
    {
        public GetBoxIdQuery(long boxId)
            
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
    }
}
