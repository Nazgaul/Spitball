namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxQuery 
    {

        public GetBoxQuery(long boxId)
        {
            BoxId = boxId;


        }
        public long BoxId { get; set; }
    }

}
