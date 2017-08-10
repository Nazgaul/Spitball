using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
            //var sw = new Stopwatch();
            //sw.Start();
            
            //Database.Initialize(false);
            //sw.Stop();
            //TraceLog.WriteInfo("time to initialize entity framework " + sw.ElapsedMilliseconds);
        }

    }
}
