using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserFlagsOthersQuery : IQueryAdmin<(IEnumerable<UserFlagsOthersDto>, int)>
    {
        public AdminUserFlagsOthersQuery(int minFlags, int page, string country)
        {
            Page = page;
            MinFlags = minFlags;
            Country = country;
        }
        public int Page { get; }
        public int MinFlags { get; }
        public string Country { get; }
    }
}
