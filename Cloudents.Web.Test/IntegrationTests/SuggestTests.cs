using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class SuggestTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_Empty_OK()
        {
            var response =
                await Client.GetAsync(
                    "/api/suggest");
            response.EnsureSuccessStatusCode();
        }
    }
}