using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

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
