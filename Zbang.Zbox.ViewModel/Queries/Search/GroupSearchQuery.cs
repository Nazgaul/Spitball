using System;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class GroupSearchQuery
    {
        public GroupSearchQuery(string query, long universityId, long userId, bool allResult) :
            this(query,universityId,userId,allResult,0)
        {
            
        }
        public GroupSearchQuery(string query, long universityId, long userId, bool allResult, int page)
        {
            if (query == null) throw new ArgumentNullException("query");
            Query = query.Replace(' ', '%');
            UniversityId = universityId;
            UserId = userId;

            if (allResult)
            {
                PageSize = 50;
            }
            else
            {
                PageSize = 6;
            }
            Offset = page * PageSize;

        }

        public string Query { get; private set; }
        public long UniversityId { get; private set; }
        public long UserId { get; private set; }
        public int Offset { get; private set; }
        public int PageSize { get; set; }
    }
}
