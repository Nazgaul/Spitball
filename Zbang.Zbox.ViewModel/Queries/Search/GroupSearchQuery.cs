using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class GroupSearchQuery
    {
        public GroupSearchQuery(string query, long universityId, long userId)
        {
            Query = query.Replace(' ', '%');
            UniversityId = universityId;
            UserId = userId;
        }

        public string Query { get; private set; }
        public long UniversityId { get; private set; }
        public long UserId { get; private set; }
        public int MaxResult { get { return 6; } }
    }
}
