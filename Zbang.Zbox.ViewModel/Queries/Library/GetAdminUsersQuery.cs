namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetAdminUsersQuery
    {
        public GetAdminUsersQuery(long universityId)
        {
            UniversityId = universityId;
        }
        public long UniversityId { get; private set; }
    }
}
