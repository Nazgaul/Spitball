using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class UrlControllerTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_NoQueryString_RedirectHomePage()
        {
            var response = await Client.GetAsync("/url").ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.OriginalString == "/");
        }


        [TestMethod]
        public async Task GetAsync_CramHost_RedirectToCram()
        {
            var url = "https%3A%2F%2Fwww.cram.com%2Fflashcards%2Faccounting-2491586";
            var response = await Client.GetAsync($"/url?url={url}&host=Cram&location=3").ConfigureAwait(false);
            var p = response.Headers.Location;

            var decode = System.Net.WebUtility.UrlDecode(url);
            Assert.IsTrue(p.OriginalString == decode);
        }

        [TestMethod]
        public async Task GetAsync_SomeGibrishUrl_HomePage()
        {
            var url =
                "/url?url=https:%2F%2Fwww.google.com&host=google' and 3481%3d3481'-- &location=23";

            var response = await Client.GetAsync(url).ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.OriginalString == "/");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAsync_NoWhiteList_500Page()
        {
            var url =
                "/url?url=https:%2F%2Fwww.google.com&host=google&location=23";

            var response = await Client.GetAsync(url).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAsync_SomeGibrishUrl2_HomePage()
        {

            var url = "/url?url=https:%2f%2fwww.google.com%00fasvp\"><a>q2ifd&host=google&location=23";
            var response = await Client.GetAsync(url).ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.OriginalString == "/");
        }

        [TestMethod]
        public async Task GetAsync_NotValidUrl_HomePage()
        {
            var url = "url?url=%2fetc%2fpasswd&host=google&location=23";
            var response = await Client.GetAsync(url).ConfigureAwait(false);
            var p = response.Headers.Location;
            Assert.IsTrue(p.OriginalString == "/");
        }
    }
}