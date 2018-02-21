using System;
using System.IO;
using System.Net.Http;
using Cloudents.MobileApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    public class ServerInit
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        public ServerInit()
        {
           // Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Startup.IntegrationTestEnvironmentName);

            //var x = Program.BuildWebHost(null);
            //_server = new TestServer(x);
            string appRootPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "Cloudents.MobileApi"));

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
