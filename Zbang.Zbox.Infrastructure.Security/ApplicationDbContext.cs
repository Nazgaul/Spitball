using Microsoft.AspNet.Identity.EntityFramework;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
