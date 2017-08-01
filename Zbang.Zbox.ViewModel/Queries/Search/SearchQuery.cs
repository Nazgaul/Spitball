using System.Globalization;
using System.Text;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public abstract class SearchQuery : IPagedQuery, IUserQuery, IQueryCache
    {
        protected SearchQuery(string term,
            long userId,
            long universityId, int pageNumber = 0, int rowsPerPage = 50)
        {
            UniversityId = universityId;
            Term = term;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            UserId = userId;
        }

        public long UserId { get; }

        public long UniversityId { get; }

        public int PageNumber { get; }


        public int RowsPerPage { get; }

        public string Term { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("user id: " + UserId);
            sb.AppendLine("university id: " + UniversityId);
            sb.AppendLine("page: " + PageNumber);
            sb.AppendLine("rows: " + RowsPerPage);
            sb.AppendLine("term: " + Term);
            return sb.ToString();
        }

        //Note search is only for the empty state
        public abstract string CacheKey {get;}
        //{
            //get { return UniversityId.ToString(CultureInfo.InvariantCulture); }
        //}

        protected string GetUniversityId()
        {
            return UniversityId.ToString(CultureInfo.InvariantCulture);
        }

        public virtual CacheRegions CacheRegion => CacheRegions.Search;

        public System.TimeSpan Expiration => System.TimeSpan.FromMinutes(20);
    }
}
