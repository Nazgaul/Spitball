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


        [TestMethod]
        public async Task GetAsync_CramHost_Redirect()
        {
            var url = "https%3A%2F%2Fwww.cram.com%2Fflashcards%2Faccounting-2491586";
            var response = await Client.GetAsync($"/url?url={url}&host=Cram&location=3").ConfigureAwait(false);
            var p = response.Headers.Location;

            var decode = System.Net.WebUtility.UrlDecode(url);
            Assert.IsTrue(p.OriginalString == decode);
        }
    }
}