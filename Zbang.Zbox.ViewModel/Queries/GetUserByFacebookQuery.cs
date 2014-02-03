
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserByFacebookQuery
    {
        public GetUserByFacebookQuery(long facebookId)
        {
            FacebookId = facebookId;
        }
        public long FacebookId { get; private set; }
    }
}
