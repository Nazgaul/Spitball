using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SearchControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public SearchControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/api/Search/documents?Format=none")]
        [InlineData("/api/search/document")]
        [InlineData("api/search/documents?query=>\"'><script>alert(72)<%2Fscript>&university=>\"'><script>alert(72)<%2Fscript>")]
        public async Task SearchDocumentAsync_Not_Found(string url)
        {
            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}