using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserVotsQuery: IQuery<IEnumerable<UserVotsDto>>
    {
        public AdminUserVotsQuery(long id, int page, int type)
        {
            Id = id;
            Page = page;
            Type = type;
        }
        public long Id { get; set; }
        public int Page { get; set; }
        public int Type { get; set; }


    }
}
