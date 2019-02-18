using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserUpVotsQuery: IQuery<IEnumerable<UserUpVotsDto>>
    {
        public AdminUserUpVotsQuery(long id, int page)
        {
            Id = id;
            Page = page;
        }
        public long Id { get; set; }
        public int Page { get; set; }

      
    }
}
