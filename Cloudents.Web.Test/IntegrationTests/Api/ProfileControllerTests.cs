using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ProfileControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public ProfileControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }


        [Theory]
        [InlineData("api/profile/159489")]
        [InlineData("api/profile/159489/about")]
        [InlineData("api/profile/159489/questions")]
        [InlineData("api/profile/159489/answers")]
        [InlineData("api/profile/159489/documents")]
        [InlineData("api/profile/159489/purchaseDocuments")]
        public async Task GetAsync_Profile_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            str.IsValidJson().Should().BeTrue();
        }

        [Theory]
        [InlineData("/api/profile/1")]
        public async Task GetAsync_Profile_NotFound(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
