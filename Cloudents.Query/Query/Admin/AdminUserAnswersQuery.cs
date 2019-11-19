using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserAnswersQuery : IQueryAdmin<IEnumerable<UserAnswersDto>>
    {
        public AdminUserAnswersQuery(long userId, int page, string country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }
        public long UserId { get; }
        public int Page { get; }
        public string Country { get; }
    }
}
