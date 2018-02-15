using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class ServerInit
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        public ServerInit()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Startup.IntegrationTestEnvironmentName);

            //var x = Program.BuildWebHost(null);
            //_server = new TestServer(x);
            string appRootPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "Cloudents.Web"));

            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(appRootPath)
                .UseConfiguration(
                    new ConfigurationBuilder()
                        .SetBasePath(appRootPath)
                        .AddJsonFile("appsettings.json")
                        .Build())
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }
    }
}
