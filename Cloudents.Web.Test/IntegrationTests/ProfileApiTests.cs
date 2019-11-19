using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ProfileApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public ProfileApiTests(SbWebApplicationFactory factory)
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
