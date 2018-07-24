using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class UrlControllerTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_NoQueryString_Redirect()
        {
            var response = await Client.GetAsync("/url").ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.OriginalString == "/");
        }
    }
}