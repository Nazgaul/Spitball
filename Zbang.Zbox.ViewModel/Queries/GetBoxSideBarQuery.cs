
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxSideBarQuery 
    {
        public GetBoxSideBarQuery(long boxId, long userId)
        {
            UserId = userId;
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
        

        public long UserId { get; private set; }
    }
}
