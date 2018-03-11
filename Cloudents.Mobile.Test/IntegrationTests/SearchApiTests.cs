using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class SearchApiTests : ServerInit
    {
        [TestMethod]
        public async Task SearchDocumentAsync_OnlyFormat_Ok()
        {
            var response = await Client.GetAsync("/api/Search/documents?Format=none").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}