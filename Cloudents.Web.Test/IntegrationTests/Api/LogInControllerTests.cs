using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class LogInControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public LogInControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        

        [Theory]
        [InlineData("api/LogIn/ValidateEmail?email=elad13@cloudents.com")]
        public async Task GetAsync_Login_OkAsync(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            str.IsValidJson().Should().BeTrue();
        }

        [Theory]
        [InlineData("api/LogIn/ValidateEmail")]
        public async Task GetAsync_Login_BadRquestAsync(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Login_With_EmailAsync()
        {
            await _client.LogInAsync();
        }
    }
}
