using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class TutorApiTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public TutorApiTests()
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

        [TestMethod]
        public async Task ReturnResult()
        {
            var response = await _client.GetAsync("/api/Tutor?term=financial accounting&sort=null&page=0").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}