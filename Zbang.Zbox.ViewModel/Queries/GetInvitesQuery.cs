
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetInvitesQuery : QueryBase
    {
        public GetInvitesQuery(long userId, int page = 0)
            : base(userId)
        {
            Page = page;
        }
        public int Page { get; private set; }
    }
}
