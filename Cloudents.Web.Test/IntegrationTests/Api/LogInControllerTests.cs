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
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }
        

        [Theory]
        [InlineData("api/LogIn/ValidateEmail?email=elad13@cloudents.com")]
        public async Task GetAsync_Login_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("api/LogIn/ValidateEmail")]
        public async Task GetAsync_Login_BadRquest(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
