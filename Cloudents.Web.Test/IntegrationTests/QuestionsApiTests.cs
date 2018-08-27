using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class QuestionsApiTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_QueryXss()
        {
            var response = await Client.GetAsync("/api/Question?term=javascript:alert(219)").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }


        [TestMethod]
        public async Task GetAsync_QueryHack()
        {
            var url = "/api/Question?term=main() { int a%3D4%2Cb%3D2%3B a%3Db<<a %2B b>>2%3B printf(\"%25d\"%2C a)%3B } a) 32 b) 2 c) 4 d) none";
            var response = await Client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

        }
    }
}