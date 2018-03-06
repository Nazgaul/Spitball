using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class SearchControllerTests : ServerInit
    {
        [TestMethod]
        public async Task SearchDocumentAsync_Empty_Ok()
        {
            var response = await Client.GetAsync("api/search/documents").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task SearchFlashcardAsync_Empty_Ok()
        {
            var response = await Client.GetAsync("api/search/flashcards").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}