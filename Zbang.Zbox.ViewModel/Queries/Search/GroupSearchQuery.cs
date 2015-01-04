using System;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class GroupSearchQuery : IUserQuery, IPagedQuery
    {
        public GroupSearchQuery(string query, long universityId, long userId, int page, int rowsPerPage)
        {
            RowsPerPage = rowsPerPage;
            if (query == null) throw new ArgumentNullException("query");
            Query = query.Replace(' ', '%');
            UniversityId = universityId;
            UserId = userId;
            PageNumber = page;
            RowsPerPage = rowsPerPage;

        }

        public string Query { get; private set; }
        public long UniversityId { get; private set; }
        public long UserId { get; private set; }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }
    }
}
