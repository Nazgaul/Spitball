using FluentAssertions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class AccountControllerTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AccountControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_Unauthorized_401()
        {
            var response = await _client.GetAsync("api/account");

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetAsync_OK_200()
        {
            string credentials = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\"}";

            await _client.PostAsync("api/LogIn",
               new StringContent(credentials, Encoding.UTF8, "application/json"));

            var response = await _client.GetAsync("api/account");

            response.StatusCode.Should().Be(200);
        }
    }
}
