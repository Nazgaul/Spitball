using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserFlagsOthersQuery : IQuery<(IEnumerable<UserFlagsOthersDto>, int)>
    {
        public AdminUserFlagsOthersQuery(int minFlags, int page)
        {
            Page = page;
            MinFlags = minFlags;
        }
        public int Page { get; set; }
        public int MinFlags { get; set; }
    }
}
