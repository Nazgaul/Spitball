using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SearchApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public SearchApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/api/Search/documents?Format=none")]
        [InlineData("/api/search/document")]
        [InlineData("/api/search/flashcards")]
        [InlineData("api/search/documents?query=>\"'><script>alert(72)<%2Fscript>&university=>\"'><script>alert(72)<%2Fscript>")]
        public async Task SearchDocumentAsync_Ok(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

     


    }
}