using System.Globalization;
using System.Text;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchBoxesQuery : SearchQuery
    {
        public SearchBoxesQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50) : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey
        {
            get { return "boxes " + GetUniversityId(); }
        }
    }
    public class SearchItemsQuery : SearchQuery
    {
        public SearchItemsQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
            : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey
        {
            get { return "items " + GetUniversityId(); }
        }
    }
    public class SearchQuizesQuery : SearchQuery
    {
        public SearchQuizesQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
            : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey
        {
            get { return "quizzes " + GetUniversityId(); }
        }
    }

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
