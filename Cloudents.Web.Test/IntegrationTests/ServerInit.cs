using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Cloudents.Web.Test.IntegrationTests
{
    //public class ServerInit : WebApplicationFactory<Cloudents.Web.Startup>
    //{
    //    protected static HttpClient Client;


    //    static ServerInit()
    //    {
    //        WebApplicationFactory<Startup> factory = new WebApplicationFactory<Startup>();

    //        Client = factory.CreateClient();
    //        //Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Startup.IntegrationTestEnvironmentName);

    //        ////var x = Program.BuildWebHost(null);
    //        ////_server = new TestServer(x);
    //        //var appRootPath = Path.GetFullPath(Path.Combine(
    //        //    AppContext.BaseDirectory,
    //        //    "..", "..", "..", "..", "Cloudents.Web"));

    //        //var server = new TestServer(new WebHostBuilder()
    //        //    .UseContentRoot(appRootPath)
    //        //    .UseConfiguration(
    //        //        new ConfigurationBuilder()
    //        //            .SetBasePath(appRootPath)
    //        //            .AddJsonFile("appsettings.json")
    //        //            .AddJsonFile("appsettings.Development.json")
    //        //            .Build())
    //        //    .UseStartup<Startup>());
    //        //Client = server.CreateClient();
    //    }
    //}
}
