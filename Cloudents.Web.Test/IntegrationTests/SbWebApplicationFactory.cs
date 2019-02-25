using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class SbWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(Startup.IntegrationTestEnvironmentName);
        }
        
    }
}