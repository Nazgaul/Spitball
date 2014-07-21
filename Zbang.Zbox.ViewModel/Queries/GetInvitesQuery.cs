
namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetInvitesQuery : QueryBase
    {
        public GetInvitesQuery(long userId)
            : this(userId, 0)
        {

        }
        public GetInvitesQuery(long userId, int page)
            : base(userId)
        {
            Page = page;
        }
        public int Page { get; private set; }
    }
}
