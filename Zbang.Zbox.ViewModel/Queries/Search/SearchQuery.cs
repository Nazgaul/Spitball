using System.Globalization;
using System.Text;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchQuery : IPagedQuery, IUserQuery, IQueryCache
    {
        public SearchQuery(string term,
            long userId,
            long universityId, int pageNumber = 0, int rowsPerPage = 50)
        {
            UniversityId = universityId;
            Term = term;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            UserId = userId;
        }

        public long UserId { get; private set; }

        public long UniversityId { get; private set; }

        public int PageNumber { get; private set; }


        public int RowsPerPage { get; private set; }

        public string Term { get; private set; }

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
        public string CacheKey
        {
            get { return UniversityId.ToString(CultureInfo.InvariantCulture); }
        }

        public virtual string CacheRegion
        {
            get { return "search"; }
        }

        public System.TimeSpan Expiration
        {
            get { return System.TimeSpan.FromMinutes(20); }
        }
    }
}
