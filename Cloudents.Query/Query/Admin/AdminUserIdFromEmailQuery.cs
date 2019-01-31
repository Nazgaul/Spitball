using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUserIdFromEmailQuery : IQuery<long>
    {
        public AdminUserIdFromEmailQuery(string email)
        {
            Email = email;
        }
        public string Email { get; set; }
    }
}
