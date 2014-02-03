namespace Zbang.Zbox.ViewModel.Queries
{
    public class QueryPagedBase : QueryBase
    {
        public QueryPagedBase(long userid, int pageNumber)
            : base(userid)
        {
            PageNumber = pageNumber;
        }
        public int PageNumber { get; set; }
    }
}
