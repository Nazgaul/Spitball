namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetHomePageQuery
    {
        public GetHomePageQuery(long? universityId)
        {
            UniversityId = universityId;
        }

        //public GetHomePageQuery(long[] boxIds)
        //{
        //    BoxIds = boxIds;
        //}

        //public long[] BoxIds { get; private set; }


        public long? UniversityId { get; private set; }
    }
}
