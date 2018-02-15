using System.Threading.Tasks;
using Cloudents.Web.Test.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Web.Test
{
    [TestClass]
    public class BookApiTests : ServerInit
    {
        [TestMethod]
        public async Task Gibrish_ReturnResult()
        {
            var response = await _client.GetAsync("api/book/search?term=kjhgfd").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}