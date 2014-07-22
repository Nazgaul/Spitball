using System;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class GroupSearchQuery
    {
        public GroupSearchQuery(string query, long universityId, long userId, bool allResult, int page = 0)
        {
            if (query == null) throw new ArgumentNullException("query");
            Query = query.Replace(' ', '%');
            UniversityId = universityId;
            UserId = userId;

            PageSize = allResult ? 50 : 6;
            Offset = page * PageSize;

        }

        public string Query { get; private set; }
        public long UniversityId { get; private set; }
        public long UserId { get; private set; }
        public int Offset { get; private set; }
        public int PageSize { get; private set; }
    }
}
