namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetHomePageQuery
    {
        public GetHomePageQuery(long? universityId)
        {
            UniversityId = universityId;
        }
        public long? UniversityId { get; private set; }
    }
}
