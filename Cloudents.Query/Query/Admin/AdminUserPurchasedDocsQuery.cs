using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserPurchasedDocsQuery : IQuery<IEnumerable<UserPurchasedDocsDto>>
    {
        public AdminUserPurchasedDocsQuery(long userId, int page)
        {
            UserId = userId;
            Page = page;
        }
        public long UserId { get; }
        public int Page { get; }
    }
}
