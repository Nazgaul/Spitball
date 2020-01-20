using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
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
        public async Task GetAsync_Profile_OkAsync(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            str.IsValidJson().Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_ImageIsValidAsync()
        {
            var response = await _client.GetAsync("api/profile/159039");

            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            dynamic json = JToken.Parse(str);
            string image = json.image;
            var uri = new Uri(image);

        }

        [Theory]
        [InlineData("/api/profile/1")]
        public async Task GetAsync_Profile_NotFoundAsync(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
