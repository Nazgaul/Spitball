using System;
using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class AdminUser
    {
        public virtual Guid Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string Country { get; set; }

       // public virtual ISet<AdminUserRoles> Roles { get; set; }
    }

    //public class AdminUserRoles
    //{
    //    public virtual Guid Id { get; set; }

    //    public virtual string Role { get; set; }
    //}

    //public class AdminRoles : Enumeration
    //{

    //    public AdminRoles(int id, string name) : base(id, name)
    //    {
    //    }
    //}
}