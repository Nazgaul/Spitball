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


    public class GetHomeBoxesUniversityQuery
    {
        public GetHomeBoxesUniversityQuery(long? universityId, string country)
        {
            UniversityId = universityId;
            Country = country;
        }

        public long? UniversityId { get; private set; }

        public string Country { get; private set; }
    }
}
