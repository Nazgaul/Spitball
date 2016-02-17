namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetHomePageQuery
    {
        public GetHomePageQuery(long[] boxIds)
        {
            BoxIds = boxIds;
        }

        public long[] BoxIds { get; private set; }
    }
}
