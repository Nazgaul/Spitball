using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class BookControllerTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BookControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAsync_Redirect_To_Default()
        {
            var response = await _client.GetAsync("book");

            var p = response.Headers.Location;

            Assert.True(p.OriginalString == "/");
        }

        [Fact]
        public async Task GetAsync_OK_200()
        {
            await _client.GetAsync("book");

            string credentials = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            await _client.PostAsync("api/LogIn", new StringContent(credentials, Encoding.UTF8, "application/json"));

            var response = await _client.GetAsync("book");

            response.StatusCode.Should().Be(200);
        }
    }
}
