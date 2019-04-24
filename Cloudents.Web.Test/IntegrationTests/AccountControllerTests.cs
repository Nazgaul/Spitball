using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class AccountControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

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
            await _client.LogInAsync();
            var response = await _client.GetAsync("api/account");

            response.StatusCode.Should().Be(200);
        }
    }
}
