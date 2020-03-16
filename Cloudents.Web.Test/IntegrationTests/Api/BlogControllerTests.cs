using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class BlogControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public BlogControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/blog")]
        [InlineData("api/marketing")]

        public async Task ChatApiTestGet_NotLogIn_UnauthorizedAsync(string api)
        {
            await _client.LogInAsync();
            var _ = await _client.GetAsync(api);
        }
    }
}