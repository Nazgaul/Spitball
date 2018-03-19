using System.Threading.Tasks;
using Cloudents.Api.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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

            var result = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<WebResponse>(result);
            obj.Result.Should().HaveCountGreaterOrEqualTo(1);
        }

        [TestMethod]
        public async Task SearchFlashcardAsync_Empty_Ok()
        {
            var response = await Client.GetAsync("api/search/flashcards").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<WebResponse>(result);
            obj.Result.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}