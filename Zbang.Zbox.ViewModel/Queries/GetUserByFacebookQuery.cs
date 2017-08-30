
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

    public class GetUserByGoogleQuery
    {
        public GetUserByGoogleQuery(string googleId)
        {
            GoogleId = googleId;
        }
        public string GoogleId { get; private set; }
    }
}
