using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class GroupSearchQuery
    {
        public GroupSearchQuery(string query, long universityId, long userId, bool AllResult = false)
        {
            Query = query.Replace(' ', '%');
            UniversityId = universityId;
            UserId = userId;

            if (AllResult)
            {
                MaxResult = int.MaxValue;
            }
            else
            {
                MaxResult = 6;
            }

        }

        public string Query { get; private set; }
        public long UniversityId { get; private set; }
        public long UserId { get; private set; }
        public int MaxResult { get; private set; }
    }
}
