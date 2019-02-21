using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserFlagsQuery : IQuery<IEnumerable<UserFlagsDto>>
    {
        public AdminUserFlagsQuery(long id, int page)
        {
            Id = id;
            Page = page;
        }
        public long Id { get; set; }
        public int Page { get; set; }
    }
}
