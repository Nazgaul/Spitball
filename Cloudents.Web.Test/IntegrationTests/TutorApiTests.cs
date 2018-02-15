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
    public class TutorApiTests : ServerInit
    {
        

        [TestMethod]
        public async Task ReturnResult()
        {
            var response = await _client.GetAsync("/api/Tutor?term=financial accounting&sort=null&page=0").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}