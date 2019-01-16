using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserInfoQuery : IQuery<UserInfoDto>
    {
        public AdminUserInfoQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }
    }
}
