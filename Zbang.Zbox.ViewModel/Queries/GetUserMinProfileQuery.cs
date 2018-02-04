namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserMinProfileQuery
    {
        public GetUserMinProfileQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; }
    }
}
