using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class DbContext : IdentityDbContext<ApplicationUser>
    {
        public DbContext() :base("Zbox")
        {
            
        }

        public static DbContext Create()
        {
            return new DbContext();
        }
    }
}
