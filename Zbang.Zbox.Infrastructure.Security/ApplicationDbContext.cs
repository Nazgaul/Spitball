using Microsoft.AspNet.Identity.EntityFramework;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(ConfigFetcher.Fetch("Zbox"))
        {

        }

    }
}
