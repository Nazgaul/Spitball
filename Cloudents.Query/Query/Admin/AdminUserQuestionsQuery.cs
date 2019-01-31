using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserQuestionsQuery : IQuery<IEnumerable<UserQuestionsDto>>
    {
        public AdminUserQuestionsQuery(long userId, int page)
        {
            UserId = userId;
            Page = page;
        }
        public long UserId { get; }
        public int Page { get; set; }
    }
}
